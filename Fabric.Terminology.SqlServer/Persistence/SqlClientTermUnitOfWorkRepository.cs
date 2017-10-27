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
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Persistence.UnitOfWork;

    internal class SqlClientTermUnitOfWorkRepository : IClientTermUnitOfWorkRepository
    {
        private readonly IClientTermUnitOfWork uow;

        public SqlClientTermUnitOfWorkRepository(IClientTermUnitOfWork uow)
        {
            this.uow = uow;
            this.Operations = new Queue<Operation>();
        }

        internal Queue<Operation> Operations { get; }

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

            this.uow.Context.ChangeTracker.AutoDetectChangesEnabled = false;
            using (var transaction = this.uow.Context.Database.BeginTransaction())
            {
                try
                {
                    this.uow.Context.ValueSetDescriptions.Add(valueSetDto);
                    this.uow.Context.ValueSetCodes.AddRange(codeDtos);
                    this.uow.Context.ValueSetCodeCounts.AddRange(countDtos);

                    var changes = this.uow.Context.SaveChanges();

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
                    this.uow.Logger.Error(ex, "Failed to save a custom ValueSet");
                    this.uow.Context.ChangeTracker.AutoDetectChangesEnabled = true;
                    return Attempt<IValueSet>.Failed(
                        new ValueSetOperationException("Failed to save a custom ValueSet", ex),
                        valueSet);
                }
                finally
                {
                    this.uow.Context.ChangeTracker.AutoDetectChangesEnabled = true;
                }
            }
        }

        public void PrepareAddCodesOperations(
            Guid valueSetGuid,
            IEnumerable<IValueSetCode> valueSetCodes)
        {
            var originalDtos = this.GetCodeDtos(valueSetGuid);

            var addOps = this.BuildAddCodeOperationList(originalDtos, valueSetCodes);

            var allCodeDtos = originalDtos.Union(addOps.Where(ao => ao.OperationType == OperationType.Create).Select(ao => ao.Value<ValueSetCodeDto>()));

            var countOps = this.BuildCodeCountOperationList(valueSetGuid, allCodeDtos);

            this.Enqueue(addOps.Union(countOps));
        }

        public void PrepareRemoveCodesOperations(
            Guid valueSetGuid,
            IEnumerable<ICodeSystemCode> codes)
        {
            var originalCodeDtos = this.GetCodeDtos(valueSetGuid);

            var removeOps = this.BuildRemoveCodesOperationList(originalCodeDtos, codes);
            var removedCodeGuids = removeOps.Select(ro => ro.Value<CodeSystemCodeDto>().CodeGUID);
            var resultDtos = originalCodeDtos.Where(oc => removedCodeGuids.All(rc => rc != oc.CodeGUID));

            var countOps = this.BuildCodeCountOperationList(valueSetGuid, resultDtos);

            this.Enqueue(removeOps.Union(countOps));
        }

        public void Delete(IValueSet valueSet)
        {
            using (var transaction = this.uow.Context.Database.BeginTransaction())
                try
                {
                    this.uow.Context.BulkDelete(
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
                    this.uow.Logger.Error(operationException, "Failed to delete custom ValueSet");
                    throw operationException;
                }
        }

        private void Enqueue(IEnumerable<Operation> operations)
        {
            foreach (var o in operations)
            {
                this.Operations.Enqueue(o);
            }
        }

        private IReadOnlyCollection<Operation> BuildAddCodeOperationList(
            IEnumerable<ValueSetCodeDto> originalCodeDtos,
            IEnumerable<IValueSetCode> valueSetCodes)
        {
            var existingGuids = originalCodeDtos.Select(eg => eg.CodeGUID);
            return valueSetCodes.Where(code => existingGuids.All(eg => eg != code.CodeGuid))
                .Select(
                    code => new Operation
                    {
                        Value = new ValueSetCodeDto(code),
                        OperationType = OperationType.Create
                    })
                .ToList();
        }

        private IReadOnlyCollection<Operation> BuildRemoveCodesOperationList(
            IEnumerable<ValueSetCodeDto> originalCodeDtos,
            IEnumerable<ICodeSystemCode> codeSystemCodes)
        {
            var destGuids = codeSystemCodes.Select(ds => ds.CodeGuid);
            return originalCodeDtos.Where(code => destGuids.All(dg => dg != code.CodeGUID))
                .Select(dto => new Operation
                {
                    Value = dto,
                    OperationType = OperationType.Delete
                }).ToList();
        }

        private IReadOnlyCollection<Operation> BuildCodeCountOperationList(
            Guid valueSetGuid,
            IEnumerable<ValueSetCodeDto> allCodeDtos)
        {
            var originalCounts = this.GetCodeCountDtos(valueSetGuid);

            var newCodes = allCodeDtos as ValueSetCodeDto[] ?? allCodeDtos.ToArray();
            var allCodeSystems = newCodes.Select(c => c.CodeSystemGuid).Distinct();

            var newCounts = allCodeSystems.Select(
                            codeSystemGuid => new
                            {
                                codeSystemGuid,
                                valueSetDto = newCodes.First(dto => dto.CodeSystemGuid == codeSystemGuid)
                            })
                            .Select(o =>
                            new ValueSetCodeCountDto(o.valueSetDto.ValueSetGUID,
                                o.valueSetDto.CodeSystemGuid,
                                o.valueSetDto.CodeSystemNM,
                                newCodes.Count(c => c.CodeSystemGuid == o.codeSystemGuid)))
                                .ToList();

            var operations = newCounts.Select(nc =>
                Maybe.From(originalCounts.FirstOrDefault(ec => ec.CodeSystemGUID == nc.CodeSystemGUID))
                .Select(
                    dto =>
                        {
                            var op = new Operation();
                            if (dto.CodeSystemPerValueSetNBR != nc.CodeSystemPerValueSetNBR)
                            {
                                dto.CodeSystemPerValueSetNBR = nc.CodeSystemPerValueSetNBR;
                                op.OperationType = OperationType.Update;
                            }
                            else
                            {
                                op.OperationType = OperationType.None;
                            }

                            op.Value = dto;
                            return op;
                        })
                .Else(() => new Operation
                {
                    Value = nc
                })).ToList();

            // finally ensure that any existing counts that were not in the new counts are removed
            // e.g. all codes from a particular code system were removed
            var removers = originalCounts.Except(newCounts);

            operations.AddRange(removers.Select(r => new Operation
            {
               Value = r,
               OperationType = OperationType.Delete
            }));

            return operations;
        }

        private Maybe<ValueSetDescriptionBaseDto> GetValueSetDescriptionDto(Guid valueSetGuid) =>
            Maybe.From(this.uow.ValueSetDescriptions.Single(vsd => vsd.ValueSetGUID == valueSetGuid));

        private IReadOnlyCollection<ValueSetCodeDto> GetCodeDtos(Guid valueSetGuid) =>
            this.uow.ValueSetCodes.Get(vsc => vsc.ValueSetGUID == valueSetGuid);

        private IReadOnlyCollection<ValueSetCodeCountDto> GetCodeCountDtos(Guid valueSetGuid) =>
            this.uow.ValueSetCodeCounts.Get(vscc => vscc.ValueSetGUID == valueSetGuid);

        private IReadOnlyCollection<IValueSetCode> GetCodes(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeFactory();
            var codes = this.GetCodeDtos(valueSetGuid);

            return codes.Select(factory.Build).ToList();
        }

        private IReadOnlyCollection<IValueSetCodeCount> GetCodeCounts(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeCountFactory();
            var counts = this.GetCodeCountDtos(valueSetGuid);

            return counts.Select(factory.Build).ToList();
        }
    }
}