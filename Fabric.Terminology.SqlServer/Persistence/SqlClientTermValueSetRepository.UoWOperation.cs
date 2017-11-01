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
            var dups = codesToAdd.Where(code => codesToRemove.Select(r => r.CodeGuid).Contains(code.CodeGuid)).ToList();
            var codeDeletes = codesToRemove.Except(dups).ToList();

            var batchInsertDtos = codesToAdd.Except(dups).Select(code => new ValueSetCodeDto(code)
                                    {
                                        ValueSetGUID = valueSetGuid
                                    }).ToList();

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
            var originalCounts = this.uowManager.GetCodeCountDtos(valueSetGuid);

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

        // add ability to remove all codes that exist in current value set that also exist in 
        // value set passed.

        private AddRemoveCodeOperations PrepareRemoveCodesOperations(
            IReadOnlyCollection<ValueSetCodeDto> currentCodeDtos,
            IEnumerable<ICodeSystemCode> codes)
        {
            var removeOps = this.BuildRemoveCodesOperationList(currentCodeDtos, codes);
            var removedCodeGuids = removeOps.Select(ro => ro.Value<CodeSystemCodeDto>().CodeGUID);
            var resultDtos = currentCodeDtos.Where(oc => removedCodeGuids.All(rc => rc != oc.CodeGUID));

            return new AddRemoveCodeOperations { CurrentCodeDtos = resultDtos.ToList(), Operations = removeOps };
        }
    }
}
