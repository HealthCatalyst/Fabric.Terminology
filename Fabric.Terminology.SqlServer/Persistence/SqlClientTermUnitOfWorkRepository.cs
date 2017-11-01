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

    internal partial class SqlClientTermUnitOfWorkRepository : IClientTermUnitOfWorkRepository
    {
        private readonly ILogger logger;

        private readonly IClientTermUnitOfWork uow;

        private readonly IClientTermCacheManager cacheManager;

        public SqlClientTermUnitOfWorkRepository(
            ILogger logger,
            IClientTermUnitOfWork uow,
            IClientTermCacheManager cacheManager)
        {
            this.logger = logger;
            this.uow = uow;
            this.cacheManager = cacheManager;
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid)
        {
            return this.uow.GetValueSetDescriptionDto(valueSetGuid)
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

            this.uow.Commit(this.PerpareNewValueSetOperations(valueSet));

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
            return this.uow.GetValueSetDescriptionDto(valueSetGuid)
                .Select(vsd =>
                {
                    if (vsd.StatusCD != ValueSetStatus.Draft.ToString())
                    {
                        return NotFoundAttempt();
                    }

                    var operations = this.PrepareAddRemoveCodes(
                        valueSetGuid,
                        codesToAdd.ToList(),
                        codesToRemove.ToList());

                    this.uow.Commit(operations);

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
            using (var transaction = this.uow.Context.Database.BeginTransaction())
            {
                try
                {
                    this.uow.Context.BulkDelete(
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

        private static Operation BuildOperation(object value, OperationType operationType) =>
            new Operation { Value = value, OperationType = operationType };

        private static IEnumerable<Operation> BuildOperationBatch(IEnumerable<object> values, OperationType operationType)
            => values.Select(v => BuildOperation(v, operationType));

        private static bool EnsureIsNew(IValueSet valueSet) =>
            valueSet.IsCustom && valueSet.IsLatestVersion && valueSet.ValueSetGuid.Equals(Guid.Empty);

        private static Queue<Operation> Enqueue(IEnumerable<Operation> operations)
        {
            var queue = new Queue<Operation>();
            foreach (var o in operations)
            {
                queue.Enqueue(o);
            }

            return queue;
        }

        private IReadOnlyCollection<IValueSetCode> GetCodes(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeFactory();
            var codes = this.uow.GetCodeDtos(valueSetGuid);
            return codes.Select(factory.Build).ToList();
        }

        private IReadOnlyCollection<IValueSetCodeCount> GetCodeCounts(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeCountFactory();
            var counts = this.uow.GetCodeCountDtos(valueSetGuid);
            return counts.Select(factory.Build).ToList();
        }
    }
}