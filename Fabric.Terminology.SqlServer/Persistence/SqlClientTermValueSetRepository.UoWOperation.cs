namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.UnitOfWork;

    internal partial class SqlClientTermValueSetRepository
    {
        private static ValueSetCodeDto ConvertToValueSetCodeDto(Guid valueSetGuid, ICodeSystemCode code)
        {
            return new ValueSetCodeDto(code) { ValueSetGUID = valueSetGuid };
        }

        private static Operation GetCodeSystemRecountOperation(ValueSetCodeCountDto original, ValueSetCodeCountDto recount)
        {
            var op = new Operation();
            if (original.CodeSystemPerValueSetNBR != recount.CodeSystemPerValueSetNBR)
            {
                original.CodeSystemPerValueSetNBR = recount.CodeSystemPerValueSetNBR;
                op.OperationType = OperationType.Update;
            }
            else
            {
                op.OperationType = OperationType.None;
            }

            op.Value = original;
            return op;
        }

        private AddRemoveCodeOperations PrepareAddRemoveCodes(
            Guid valueSetGuid,
            IReadOnlyCollection<ICodeSystemCode> codesToAdd,
            IReadOnlyCollection<ICodeSystemCode> codesToRemove)
        {
            var currentCodeDtos = this.uowManager.GetCodeDtos(valueSetGuid);

            //// There could be duplicates between the codesToAdd and codesToRemove collections.
            //// In cases where a code is found in both lists, it will simply be removed from each list
            //// since we are using SqlBulkCopy to batch insert records the "added" codes (e.g. there is no
            //// update/delete capability in that process and we want to ensure we do not insert duplicate codes)

            // dups are codes that would be removed and then immediately added again.
            var removeCodeGuids = codesToRemove.Select(r => r.CodeGuid).ToHashSet();

            var dups = codesToAdd.Where(code => removeCodeGuids.Contains(code.CodeGuid))
                .Select(code => code.CodeGuid)
                .ToList();

            if (dups.Any())
            {
                var dupException = new InvalidOperationException($"Attempt to both Add and Remove duplicate {dups.Count} codes to value set with id: {valueSetGuid}");
                this.logger.Error(dupException, "Cannot add and remove duplicate codes");

                // throw early
                throw dupException;
            }

            var codeDeletes = codesToRemove.Where(code => !dups.Contains(code.CodeGuid)).ToList();

            var batchInsertDtos = codesToAdd.Where(code => !dups.Contains(code.CodeGuid))
                .Select(code => ConvertToValueSetCodeDto(valueSetGuid, code))
                .ToList();

            var removeResult = this.PrepareRemoveCodesOperations(currentCodeDtos, codeDeletes);

            var finalCodes = removeResult.CurrentCodeDtos.Union(batchInsertDtos);

            return new AddRemoveCodeOperations
            {
                CurrentCodeDtos = currentCodeDtos,
                NewCodeDtos = batchInsertDtos,
                Operations = removeResult.Operations.Union(this.BuildCodeCountOperationList(valueSetGuid, finalCodes))
                    .ToList()
            };
        }

        private IReadOnlyCollection<Operation> BuildRemoveCodesOperationList(
            IEnumerable<ValueSetCodeDto> originalCodeDtos,
            IEnumerable<ICodeSystemCode> removeCodes)
        {
            var removeGuids = removeCodes.Select(ds => ds.CodeGuid).ToHashSet();
            return originalCodeDtos.Where(code => removeGuids.Contains(code.CodeGUID.GetValueOrDefault()))
                .Select(dto => new Operation { Value = dto, OperationType = OperationType.Delete })
                .ToList();
        }

        private IReadOnlyCollection<Operation> BuildCodeCountOperationList(
            Guid valueSetGuid,
            IEnumerable<ValueSetCodeDto> allCodeDtos)
        {
            var originalCounts = this.uowManager.GetCodeCountDtos(valueSetGuid);
            var existingCodeSystems = originalCounts.Select(c => c.CodeSystemGUID);

            var allCodesByCodeSystem = allCodeDtos.ToLookup(c => c.CodeSystemGuid);
            var recounts = (from g in allCodesByCodeSystem
                            let valueSetDto = g.First()
                            select new ValueSetCodeCountDto(
                                valueSetDto.ValueSetGUID,
                                valueSetDto.CodeSystemGuid,
                                valueSetDto.CodeSystemNM,
                                g.Count())).ToList();

            var operations =
                recounts.Select(
                    recount =>
                    originalCounts.Where(oc => oc.CodeSystemGUID == recount.CodeSystemGUID)
                        .FirstMaybe()
                        .Select(dto => GetCodeSystemRecountOperation(dto, recount))
                        .Else(() => new Operation { Value = recount }))
                        .Where(operation => operation.OperationType != OperationType.None)
                        .ToList();

            // finally ensure that any existing counts that were not in the new counts are removed
            // e.g. all codes from a particular code system were removed
            var removers = originalCounts.Where(oc => !existingCodeSystems.Contains(oc.CodeSystemGUID));

            operations.AddRange(
                removers.Select(r => new Operation { Value = r, OperationType = OperationType.Delete }));

            return operations.ToList();
        }

        //// add ability to remove all codes that exist in current value set that also exist in
        //// value set passed.

        private AddRemoveCodeOperations PrepareRemoveCodesOperations(
            IReadOnlyCollection<ValueSetCodeDto> currentCodeDtos,
            IEnumerable<ICodeSystemCode> removeCodes)
        {
            var removeOps = this.BuildRemoveCodesOperationList(currentCodeDtos, removeCodes);
            var removedCodeGuids = removeOps.Select(ro => ro.Value<ValueSetCodeDto>().CodeGUID);
            var resultDtos = currentCodeDtos.Where(oc => !removedCodeGuids.Contains(oc.CodeGUID));

            return new AddRemoveCodeOperations { CurrentCodeDtos = resultDtos.ToList(), Operations = removeOps };
        }
    }
}