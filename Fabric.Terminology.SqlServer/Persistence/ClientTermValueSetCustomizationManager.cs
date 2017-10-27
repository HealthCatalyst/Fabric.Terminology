namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    internal class ClientTermValueSetCustomizationManager : IClientTermValueSetCustomizationManager
    {
        private readonly Lazy<ClientTermContext> clientTermContext;

        public ClientTermValueSetCustomizationManager(Lazy<ClientTermContext> clientTermContext)
        {
            this.clientTermContext = clientTermContext;
        }

        public IReadOnlyCollection<RepositoryOperation> GetRemoveCodesOperations(
            Guid valueSetGuid,
            IEnumerable<ICodeSystemCode> codes)
        {
            var originalCodeDtos = this.GetCodeDtos(valueSetGuid);

            var removeOps = this.BuildRemoveCodesOperationList(originalCodeDtos, codes);
            var removedCodeGuids = removeOps.Select(ro => ro.Value<CodeSystemCodeDto>().CodeGUID);
            var resultDtos = originalCodeDtos.Where(oc => removedCodeGuids.All(rc => rc != oc.CodeGUID));

            var countOps = this.BuildCodeCountOperationList(valueSetGuid, resultDtos);

            return removeOps.Union(countOps).ToList();
        }

        public IReadOnlyCollection<RepositoryOperation> GetAddCodesOperations(
            Guid valueSetGuid,
            IEnumerable<IValueSetCode> valueSetCodes)
        {
            var originalDtos = this.GetCodeDtos(valueSetGuid);

            var addOps = this.BuildAddCodeOperationList(originalDtos, valueSetCodes);

            var allCodeDtos = originalDtos.Union(addOps.Where(ao => ao.OperationType == OperationType.Create).Select(ao => ao.Value<ValueSetCodeDto>()));

            var countOps = this.BuildCodeCountOperationList(valueSetGuid, allCodeDtos);

            return addOps.Union(countOps).ToList();
        }

        private IReadOnlyCollection<RepositoryOperation> BuildAddCodeOperationList(
            IEnumerable<ValueSetCodeDto> originalCodeDtos,
            IEnumerable<IValueSetCode> valueSetCodes)
        {
            var existingGuids = originalCodeDtos.Select(eg => eg.CodeGUID);
            return valueSetCodes.Where(code => existingGuids.All(eg => eg != code.CodeGuid))
                .Select(
                    code => new RepositoryOperation
                    {
                        Value = new ValueSetCodeDto(code),
                        OperationType = OperationType.Create
                    })
                .ToList();
        }

        private IReadOnlyCollection<RepositoryOperation> BuildRemoveCodesOperationList(
            IEnumerable<ValueSetCodeDto> originalCodeDtos,
            IEnumerable<ICodeSystemCode> codeSystemCodes)
        {
            var destGuids = codeSystemCodes.Select(ds => ds.CodeGuid);
            return originalCodeDtos.Where(code => destGuids.All(dg => dg != code.CodeGUID))
                .Select(dto => new RepositoryOperation
                {
                    Value = dto,
                    OperationType = OperationType.Delete
                }).ToList();
        }

        private IReadOnlyCollection<RepositoryOperation> BuildCodeCountOperationList(
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
                                newCodes.Count(c => c.CodeSystemGuid == o.codeSystemGuid)));

            return newCounts.Select(nc =>
                Maybe.From(originalCounts.FirstOrDefault(ec => ec.CodeSystemGUID == nc.CodeSystemGUID))
                .Select(
                    dto =>
                        {
                            var op = new RepositoryOperation();
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
                .Else(() => new RepositoryOperation
                {
                    Value = nc
                })).ToList();
        }

        private IReadOnlyCollection<ValueSetCodeDto> GetCodeDtos(Guid valueSetGuid) => this.clientTermContext.Value.ValueSetCodes.Where(vsc => vsc.ValueSetGUID == valueSetGuid).ToList();

        private IReadOnlyCollection<ValueSetCodeCountDto> GetCodeCountDtos(Guid valueSetGuid) =>
            this.clientTermContext.Value.ValueSetCodeCounts.Where(vscc => vscc.ValueSetGUID == valueSetGuid).ToList();
    }
}