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
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    internal class ValueSetUpdateUnitOfWorkBuilder : IValueSetUpdateUnitOfWorkBuilder
    {
        private readonly ClientTermContext clientTermContext;

        public ValueSetUpdateUnitOfWorkBuilder(ClientTermContext clientTermContext)
        {
            this.clientTermContext = clientTermContext;
        }

        public IUnitOfWork BuildAddCodesUnitOfWork(
            Guid valueSetGuid,
            IEnumerable<IValueSetCode> valueSetCodes)
        {
            var originalDtos = this.GetCodeDtos(valueSetGuid);

            var addOps = this.BuildAddCodeOperationList(originalDtos, valueSetCodes);

            var allCodeDtos = originalDtos.Union(addOps.Where(ao => ao.OperationType == OperationType.Create).Select(ao => ao.Value<ValueSetCodeDto>()));

            var countOps = this.BuildCodeCountOperationList(valueSetGuid, allCodeDtos);

            return this.Create(addOps.Union(countOps));
        }

        public IUnitOfWork BuildRemoveCodesUnitOfWork(
            Guid valueSetGuid,
            IEnumerable<ICodeSystemCode> codes)
        {
            var originalCodeDtos = this.GetCodeDtos(valueSetGuid);

            var removeOps = this.BuildRemoveCodesOperationList(originalCodeDtos, codes);
            var removedCodeGuids = removeOps.Select(ro => ro.Value<CodeSystemCodeDto>().CodeGUID);
            var resultDtos = originalCodeDtos.Where(oc => removedCodeGuids.All(rc => rc != oc.CodeGUID));

            var countOps = this.BuildCodeCountOperationList(valueSetGuid, resultDtos);

            return this.Create(removeOps.Union(countOps));
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

        private IReadOnlyCollection<ValueSetCodeDto> GetCodeDtos(Guid valueSetGuid) => 
            this.clientTermContext.ValueSetCodes.Where(vsc => vsc.ValueSetGUID == valueSetGuid).ToList();

        private IReadOnlyCollection<ValueSetCodeCountDto> GetCodeCountDtos(Guid valueSetGuid) =>
            this.clientTermContext.ValueSetCodeCounts.Where(vscc => vscc.ValueSetGUID == valueSetGuid).ToList();

        private IUnitOfWork Create(IEnumerable<Operation> operations)
        {
            var uow = new ValueSetUpdateUnitOfWork(this.clientTermContext);
            uow.Add(operations);
            return uow;
        }
    }
}