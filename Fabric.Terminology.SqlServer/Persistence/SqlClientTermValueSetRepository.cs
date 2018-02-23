namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Persistence.UnitOfWork;

    using Serilog;

    using static Fabric.Terminology.Domain.Persistence.OperationHelper;

    internal partial class SqlClientTermValueSetRepository : IClientTermValueSetRepository
    {
        private readonly ILogger logger;

        private readonly IClientTermValueUnitOfWorkManager uowManager;

        private readonly IValueSetUpdateValidationPolicy valueSetUpdateValidationPolicy;

        private readonly IClientTermCacheManager cacheManager;

        public SqlClientTermValueSetRepository(
            ILogger logger,
            IClientTermValueUnitOfWorkManager uowManager,
            IValueSetUpdateValidationPolicy valueSetUpdateValidationPolicy,
            IClientTermCacheManager cacheManager)
        {
            this.logger = logger;
            this.uowManager = uowManager;
            this.valueSetUpdateValidationPolicy = valueSetUpdateValidationPolicy;
            this.cacheManager = cacheManager;
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid)
        {
            return this.uowManager.GetValueSetDescriptionDto(valueSetGuid)
                .Select(
                    dto =>
                        {
                            var factory = new ValueSetBackingItemFactory();
                            var item = factory.Build(dto, true);
                            var codes = this.GetCodes(item.ValueSetGuid);
                            var counts = this.GetCodeCounts(item.ValueSetGuid);

                            return new ValueSet(item, codes, counts) as IValueSet;
                        });
        }

        public Attempt<IValueSet> Add(IValueSet valueSet)
        {
            if (!EnsureIsNew(valueSet))
            {
                var invalid = new InvalidOperationException("Cannot save an existing value set as a new value set.");
                return Attempt<IValueSet>.Failed(invalid);
            }

            var valueSetGuid = valueSet.SetIdsForCustomInsert();

            var descCountOps =
                CreateOperation(new ValueSetDescriptionBaseDto(valueSet), OperationType.Create)
                .AppendOperationBatch(
                    valueSet.CodeCounts.Select(count => new ValueSetCodeCountDto(count)),
                        OperationType.Create);

            // Insert ValueSetDescriptionBASE and ValueSetCodeCountBASE
            var uowDescCount = this.uowManager.CreateUnitOfWork(descCountOps);
            uowDescCount.Commit();

            // Bulk Insert ValueSetCodeBASE
            var codesDtos = valueSet.ValueSetCodes.Select(code => new ValueSetCodeDto(code)).ToList();
            var uowCounts = this.uowManager.CreateBulkCopyUnitOfWork(codesDtos);
            uowCounts.Commit();

            // Get the updated ValueSet
            var added = this.GetValueSet(valueSetGuid);

            return added.Select(Attempt<IValueSet>.Successful)
                .Else(
                    () => Attempt<IValueSet>.Failed(
                        new ValueSetNotFoundException($"Could not retrieve newly saved ValueSet {valueSetGuid}")));
        }

        public Attempt<IValueSet> Patch(ValueSetPatchParameters parameters)
        {
            return this.AttemptValueSetAlteration(parameters.ValueSetGuid, PatchAlteration);

            void PatchAlteration(ValueSetDescriptionBaseDto vsd)
            {
                if (GetValueSetStatus(vsd.StatusCD) != ValueSetStatus.Draft)
                {
                    throw new ValueSetOperationException(
                        $"Could not add or remove codes from ValueSet.  ValueSet must have a status of `{ValueSetStatus.Draft.ToString()}`.  ValueSet had a status of {vsd.StatusCD}");
                }

                vsd.SourceDSC = parameters.ValueSetMeta.SourceDescription;
                vsd.ValueSetNM = parameters.Name;
                vsd.AuthorityDSC = parameters.ValueSetMeta.AuthorityDescription;
                vsd.ClientCD = parameters.ValueSetMeta.ClientCode;
                vsd.DefinitionDSC = parameters.ValueSetMeta.DefinitionDescription;

                var updateOp = new Operation { OperationType = OperationType.Update, Value = vsd };

                var operations = new List<Operation> { updateOp };

                var work = this.PrepareAddRemoveCodes(
                    parameters.ValueSetGuid,
                    parameters.CodesToAdd.ToList(),
                    parameters.CodesToRemove.ToList());

                operations.AddRange(work.Operations);

                var uow = this.uowManager.CreateUnitOfWork(operations);
                uow.Commit();

                var bulkInsertUow = this.uowManager.CreateBulkCopyUnitOfWork(work.NewCodeDtos);
                bulkInsertUow.Commit();
            }
        }

        public Attempt<IValueSet> AddRemoveCodes(
            Guid valueSetGuid,
            IReadOnlyCollection<ICodeSystemCode> codesToAdd,
            IReadOnlyCollection<ICodeSystemCode> codesToRemove)
        {
            return this.AttemptValueSetAlteration(valueSetGuid, Alteration);

            void Alteration(ValueSetDescriptionBaseDto vsd)
            {
                if (GetValueSetStatus(vsd.StatusCD) != ValueSetStatus.Draft)
                {
                    throw new ValueSetOperationException(
                        $"Could not add or remove codes from ValueSet.  ValueSet must have a status of `{ValueSetStatus.Draft.ToString()}`.  ValueSet had a status of {vsd.StatusCD}");
                }

                var work = this.PrepareAddRemoveCodes(
                    valueSetGuid,
                    codesToAdd.ToList(),
                    codesToRemove.ToList());

                var uow = this.uowManager.CreateUnitOfWork(work.Operations);
                uow.Commit();

                var bulkInsertUow = this.uowManager.CreateBulkCopyUnitOfWork(work.NewCodeDtos);
                bulkInsertUow.Commit();
            }
        }

        public Attempt<IValueSet> ChangeStatus(Guid valueSetGuid, ValueSetStatus newStatus)
        {
            return this.AttemptValueSetAlteration(valueSetGuid, ChangeStatusAlteration);

            void ChangeStatusAlteration(ValueSetDescriptionBaseDto vsd)
            {
                var currentStatus = GetValueSetStatus(vsd.StatusCD);
                if (!this.valueSetUpdateValidationPolicy.CanChangeStatus(currentStatus, newStatus))
                {
                    throw new ValueSetOperationException($"ValueSet status policy does not allow changing the status of a ValueSet from {currentStatus.ToString()} to {newStatus.ToString()}");
                }

                vsd.StatusCD = newStatus.ToString();
                var operation = new Operation { OperationType = OperationType.Update, Value = vsd };
                var uow = this.uowManager.CreateUnitOfWork(operation);
                uow.Commit();
            }
        }

        public void Delete(IValueSet valueSet)
        {
            if (!this.valueSetUpdateValidationPolicy.CanBeDeleted(valueSet))
            {
                var statusEx = new ValueSetOperationException($"Could not delete ValueSet with ID {valueSet.ValueSetGuid}.  ValueSet status must be `{ValueSetStatus.Draft.ToString()}`, found: {valueSet.StatusCode.ToString()}");
                this.logger.Error(statusEx, "Failed to delete custom ValueSet.");
                throw statusEx;
            }

            using (var transaction = this.uowManager.Context.Database.BeginTransaction())
            {
                try
                {
                    this.uowManager.Context.BulkDelete(
                        new[] { typeof(ValueSetDescriptionBaseDto), typeof(ValueSetCodeDto), typeof(ValueSetCodeCountDto) },
                        new Dictionary<string, object>
                        {
                            { nameof(ValueSetDescriptionBaseDto.ValueSetGUID), valueSet.ValueSetGuid }
                        });
                    transaction.Commit();

                    this.cacheManager.Clear(valueSet.ValueSetGuid);
                }
                catch (Exception ex)
                {
                    var operationException = new ValueSetOperationException(
                        $"Failed to delete custom ValueSet with ID {valueSet.ValueSetGuid}",
                        ex);
                    this.logger.Error(operationException, "Failed to delete custom ValueSet");
                    throw operationException;
                }
            }
        }

        private static bool EnsureIsNew(IValueSet valueSet) =>
            valueSet.IsCustom && valueSet.IsLatestVersion && valueSet.ValueSetGuid.Equals(Guid.Empty);

        private static ValueSetStatus GetValueSetStatus(string statusString) =>
            (ValueSetStatus)Enum.Parse(typeof(ValueSetStatus), statusString, true);

        private Attempt<IValueSet> AttemptValueSetAlteration(
            Guid valueSetGuid,
            Action<ValueSetDescriptionBaseDto> doAlteration)
        {
            return this.uowManager.GetValueSetDescriptionDto(valueSetGuid)
                .Select(vsd =>
                    {
                        try
                        {
                            doAlteration(vsd);

                            this.cacheManager.Clear(valueSetGuid);

                            return this.GetValueSet(valueSetGuid)
                                .Select(Attempt<IValueSet>.Successful)
                                .Else(() => Attempt<IValueSet>.Failed(
                                    new ValueSetNotFoundException($"Could not retrieve updated ValueSet with ID {valueSetGuid}")));
                        }
                        catch (Exception ex)
                        {
                            return Attempt<IValueSet>.Failed(ex);
                        }
                    })
                .Else(NotFoundAttempt);

            Attempt<IValueSet> NotFoundAttempt() =>
                Attempt<IValueSet>.Failed(new ValueSetNotFoundException($"A customizable (ClientTerm) value set in with ValueSetGuid {valueSetGuid} could not be found. Published Shared Terminology value sets are may not be altered via this API."));
        }

        private IReadOnlyCollection<IValueSetCode> GetCodes(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeFactory();
            var codes = this.uowManager.GetCodeDtos(valueSetGuid);
            return codes.Select(factory.Build).ToList();
        }

        private IReadOnlyCollection<IValueSetCodeCount> GetCodeCounts(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeCountFactory();
            var counts = this.uowManager.GetCodeCountDtos(valueSetGuid);
            return counts.Select(factory.Build).ToList();
        }
    }
}