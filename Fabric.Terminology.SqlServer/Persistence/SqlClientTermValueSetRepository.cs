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

        private readonly IClientTermCacheManager cacheManager;

        public SqlClientTermValueSetRepository(
            ILogger logger,
            IClientTermValueUnitOfWorkManager uowManager,
            IClientTermCacheManager cacheManager)
        {
            this.logger = logger;
            this.uowManager = uowManager;
            this.cacheManager = cacheManager;
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid)
        {
            return this.uowManager.GetValueSetDescriptionDto(valueSetGuid)
                .Select(
                    dto =>
                        {
                            var factory = new ValueSetBackingItemFactory();
                            var item = factory.Build(dto);
                            ((ValueSetBackingItem)item).IsCustom = true;
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
                        new ValueSetNotFoundException("Could not retrieved newly saved ValueSet")));
        }

        public Attempt<IValueSet> AddRemoveCodes(
            Guid valueSetGuid,
            IEnumerable<ICodeSystemCode> codesToAdd,
            IEnumerable<ICodeSystemCode> codesToRemove)
        {
            return this.uowManager.GetValueSetDescriptionDto(valueSetGuid)
                .Select(vsd =>
                {
                    if (vsd.StatusCD != ValueSetStatus.Draft.ToString())
                    {
                        return NotFoundAttempt();
                    }

                    var work = this.PrepareAddRemoveCodes(
                        valueSetGuid,
                        codesToAdd.ToList(),
                        codesToRemove.ToList());

                    var uow = this.uowManager.CreateUnitOfWork(work.Operations);
                    uow.Commit();

                    var bulkInsertUow = this.uowManager.CreateBulkCopyUnitOfWork(work.NewCodeDtos);
                    bulkInsertUow.Commit();

                    this.cacheManager.Clear(valueSetGuid);

                    return this.GetValueSet(valueSetGuid)
                        .Select(Attempt<IValueSet>.Successful)
                        .Else(() => Attempt<IValueSet>.Failed(
                            new ValueSetNotFoundException("Could not retrieved updated ValueSet")));

                })
                .Else(NotFoundAttempt);

            Attempt<IValueSet> NotFoundAttempt() =>
                Attempt<IValueSet>.Failed(new ValueSetNotFoundException($"A value set in 'Draft' status with ValueSetGuid {valueSetGuid} could not be found."));
        }

        public void Delete(IValueSet valueSet)
        {
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