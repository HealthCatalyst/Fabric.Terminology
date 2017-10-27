namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Factories;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlClientTermValueSetRepository : IClientTermValueSetRepository
    {
        private readonly Lazy<ClientTermContext> clientTermContext;

        private readonly ILogger logger;

        public SqlClientTermValueSetRepository(
            Lazy<ClientTermContext> clientTermContext,
            ILogger logger)
        {
            this.clientTermContext = clientTermContext;
            this.logger = logger;
        }

        protected ClientTermContext ClientTermContext => this.clientTermContext.Value;

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid)
        {
            return this.GetValueSetDescriptionDto(valueSetGuid)
                .Select(
                    dto =>
                        {
                            var factory = new ValueSetBackingItemFactory();
                            var item = factory.Build(dto);

                            var codes = this.GetCodes(item.ValueSetGuid);
                            var counts = this.GetCodeCounts(item.ValueSetGuid);

                            return new ValueSet(item, codes, counts) as IValueSet;
                        });
        }

        public Attempt<IValueSet> Add(IValueSet valueSet)
        {
            valueSet.SetIdsForCustomInsert();

            var valueSetDto = new ValueSetDescriptionBaseDto(valueSet);
            var codeDtos = valueSet.ValueSetCodes.Select(code => new ValueSetCodeDto(code)).ToList();
            var countDtos = valueSet.CodeCounts.Select(count => new ValueSetCodeCountDto(count)).ToList();

            this.ClientTermContext.ChangeTracker.AutoDetectChangesEnabled = false;
            using (var transaction = this.ClientTermContext.Database.BeginTransaction())
            {
                try
                {
                    this.ClientTermContext.ValueSetDescriptions.Add(valueSetDto);
                    this.ClientTermContext.ValueSetCodes.AddRange(codeDtos);
                    this.ClientTermContext.ValueSetCodeCounts.AddRange(countDtos);

                    var changes = this.ClientTermContext.SaveChanges();

                    var expectedChanges = codeDtos.Count + countDtos.Count + 1;
                    if (changes != expectedChanges)
                    {
                        transaction.Rollback();
                        return Attempt<IValueSet>.Failed(
                            new ValueSetNotFoundException(
                                $"When saving a ValueSet, we expected {expectedChanges} changes, but was told there were {changes} changes"));
                    }

                    transaction.Commit();

                    // Get the updated ValueSet
                    var added = this.GetValueSet(valueSetDto.ValueSetGUID);

                    return added.Select(Attempt<IValueSet>.Successful)
                        .Else(
                            () => Attempt<IValueSet>.Failed(
                                new ValueSetNotFoundException("Could not retrieved newly saved ValueSet")));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    this.logger.Error(ex, "Failed to save a custom ValueSet");
                    this.ClientTermContext.ChangeTracker.AutoDetectChangesEnabled = true;
                    return Attempt<IValueSet>.Failed(
                        new ValueSetOperationException("Failed to save a custom ValueSet", ex),
                        valueSet);
                }
                finally
                {
                    this.ClientTermContext.ChangeTracker.AutoDetectChangesEnabled = true;
                }
            }
        }

        public void AddCodes(Guid valueSetGuid, IEnumerable<ICodeSystemCode> codeSystemCodes)
        {
            var systemCodes = codeSystemCodes as ICodeSystemCode[] ?? codeSystemCodes.ToArray();
            var existingCodes = this.GetCodes(valueSetGuid);

            var existingCodeGuids = existingCodes.Select(c => c.CodeGuid);

            var newCodes = systemCodes
                            .Where(c => existingCodeGuids.All(eg => eg != c.CodeGuid))
                            .Select(uc => new ValueSetCode(uc) { ValueSetGuid = valueSetGuid });

            //var countDtoOps = this.GetCodeCountOperations(valueSetGuid, existingCodes.Union(newCodes).ToList());
        }

        public void RemoveCodes(Guid valueSetGuid, IEnumerable<Guid> codeGuids)
        {
            throw new NotImplementedException();
        }

        public Attempt<IValueSet> Update(IValueSet valueSet)
        {
            return this.GetValueSetDescriptionDto(valueSet.ValueSetGuid)
                .Select(dto => this.PerformUpdate(dto, valueSet))
                .Else(AttemptNotFound(valueSet.ValueSetGuid));
        }

        public void Delete(IValueSet valueSet)
        {
            using (var transaction = this.ClientTermContext.Database.BeginTransaction())
            try
            {
                this.ClientTermContext.BulkDelete(
                    new[] { typeof(ValueSetDescriptionBaseDto), typeof(ValueSetCodeDto), typeof(ValueSetCodeCountDto) },
                    new Dictionary<string, object>
                    {
                        { nameof(ValueSetDescriptionBaseDto.ValueSetGUID), valueSet.ValueSetGuid }
                    });
                transaction.Commit();
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

        private static Attempt<IValueSet> AttemptNotFound(Guid valueSetGuid)
        {
            var notFound = new ValueSetNotFoundException($"Could not find ClientTerm value set with ValueSetGuid {valueSetGuid}");
            return Attempt<IValueSet>.Failed(notFound);
        }

        private Attempt<IValueSet> PerformUpdate(ValueSetDescriptionBaseDto existing, IValueSet updated)
        {
            throw new NotImplementedException();
        }

        private IReadOnlyCollection<IValueSetCode> GetCodes(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeFactory();
            var codes = this.ClientTermContext.ValueSetCodes.Where(vsc => vsc.ValueSetGUID == valueSetGuid).ToList();

            return codes.Select(factory.Build).ToList();
        }

        private IReadOnlyCollection<PersistenceOperation> GetRemoveCodeOperations(
            IEnumerable<ValueSetCodeDto> originalSet,
            IEnumerable<IValueSetCode> destinationSet)
        {
            var destGuids = destinationSet.Select(ds => ds.CodeGuid);
            return originalSet.Where(code => destGuids.All(dg => dg != code.CodeGUID))
                .Select(dto => new PersistenceOperation
                {
                    Value = dto,
                    OperationType = OperationType.Delete
                }).ToList();
        }

        private IReadOnlyCollection<PersistenceOperation> GetAddCodeOperations(
            IEnumerable<ValueSetCodeDto> originalSet,
            IEnumerable<IValueSetCode> destinationSet)
        {
            var existingGuids = originalSet.Select(eg => eg.CodeGUID);
            return destinationSet.Where(code => existingGuids.All(eg => eg != code.CodeGuid))
                .Select(
                    code => new PersistenceOperation
                    {
                        Value = new ValueSetCodeDto(code),
                        OperationType = OperationType.Create
                    })
                .ToList();
        }

        private IReadOnlyCollection<PersistenceOperation> GetCodeCountOperations(
            IEnumerable<ValueSetCodeCountDto> existingCounts,
            IEnumerable<IValueSetCode> valueSetCodes)
        {
            var newCounts = valueSetCodes.GetCodeCountsFromCodes();
            return newCounts.Select(nc =>
                    Maybe.From(existingCounts.FirstOrDefault(ec => ec.CodeSystemGUID == nc.CodeSystemGuid))
                    .Select(
                            dto =>
                            {
                                var op = new PersistenceOperation();
                                if (dto.CodeSystemPerValueSetNBR != nc.CodeCount)
                                {
                                    dto.CodeSystemPerValueSetNBR = nc.CodeCount;
                                    op.OperationType = OperationType.Update;
                                }
                                else
                                {
                                    op.OperationType = OperationType.None;
                                }

                                op.Value = dto;
                                return op;
                            })
                    .Else(() => new PersistenceOperation
                        {
                            Value = new ValueSetCodeCountDto(nc)
                        })).ToList();
        }

        private IReadOnlyCollection<IValueSetCodeCount> GetCodeCounts(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeCountFactory();
            var counts = this.ClientTermContext.ValueSetCodeCounts.Where(vscc => vscc.ValueSetGUID == valueSetGuid)
                .ToList();

            return counts.Select(factory.Build).ToList();
        }

        private Maybe<ValueSetDescriptionBaseDto> GetValueSetDescriptionDto(Guid valueSetGuid)
        {
            return Maybe.From(this.ClientTermContext.ValueSetDescriptions.AsNoTracking()
                .SingleOrDefault(x => x.ValueSetGUID == valueSetGuid));
        }
    }
}