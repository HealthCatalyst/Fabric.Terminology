namespace Fabric.Terminology.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;

    public class ClientTermCustomizationService : IClientTermCustomizationService
    {
        private readonly ICodeSystemCodeService codeSystemCodeService;

        private readonly IValueSetCodeService valueSetCodeService;

        private readonly IClientTermValueSetService clientTermValueSetService;

        public ClientTermCustomizationService(
            ICodeSystemCodeService codeSystemCodeService,
            IValueSetCodeService valueSetCodeService,
            IClientTermValueSetService clientTermValueSetService)
        {
            this.codeSystemCodeService = codeSystemCodeService;
            this.valueSetCodeService = valueSetCodeService;
            this.clientTermValueSetService = clientTermValueSetService;
        }

        public Attempt<IValueSet> CreateValueSet(ClientTermValueSetApiModel model)
        {
            var parameters = this.Build(model);
            if (parameters.IsValid())
            {
                return this.clientTermValueSetService.Create(
                    parameters.Name,
                    parameters.ValueSetMeta,
                    parameters.CodesToAdd.ToList());
            }

            var invalidModel = new InvalidOperationException("Failed to validate ClientTermValueSetApiModel. Possible duplicate codes detected between add and remove instructions.");
            return Attempt<IValueSet>.Failed(invalidModel);
        }

        public Attempt<IValueSet> UpdateValueSet(Guid valueSetGuid, ClientTermValueSetApiModel model)
        {
            var parameters = this.Build(model);
            parameters.ValueSetGuid = valueSetGuid;

            if (parameters.IsValid())
            {
                return this.clientTermValueSetService.Patch(parameters);
            }

            var invalidModel = new InvalidOperationException("Failed to validate ClientTermValueSetApiModel.  Possible duplicate codes detected between add and remove instructions.");
            return Attempt<IValueSet>.Failed(invalidModel);
        }

        private static IEnumerable<Guid> GetIdGuids(
            IEnumerable<CodeOperation> codeInstructions,
            CodeOperationSource codeOperationSource,
            OperationInstruction operationInstruction)
        {
            return codeInstructions.Where(ci => ci.Instruction == operationInstruction && ci.Source == codeOperationSource)
                .Select(ci => ci.Value);
        }

        private ValueSetPatchParameters Build(ClientTermValueSetApiModel model)
        {
            var codesToAdd = this.GetCodesForInstruction(model, OperationInstruction.Add);
            var codesToRemove = this.GetCodesForInstruction(model, OperationInstruction.Remove);

            return new ValueSetPatchParameters
            {
                Name = model.Name,
                ValueSetMeta = model,
                CodesToAdd = codesToAdd,
                CodesToRemove = codesToRemove
            };
        }

        private IEnumerable<ICodeSystemCode> GetCodesForInstruction(
            ClientTermValueSetApiModel model,
            OperationInstruction operationInstruction)
        {
            var codes = new List<ICodeSystemCode>();

            var valueSetGuids = GetIdGuids(model.CodeOperations, CodeOperationSource.ValueSet, operationInstruction);
            var codeGuids = GetIdGuids(model.CodeOperations, CodeOperationSource.CodeSystemCode, operationInstruction);

            codes.AddRange(this.GetCodeSystemCodesByValueSets(valueSetGuids));
            codes.AddRange(this.GetCodeSystemCodes(codeGuids));

            return codes.GroupBy(csc => csc.CodeGuid)
                .Select(group => group.FirstOrDefault())
                .Where(csc => csc != null)
                .ToList();
        }

        private IEnumerable<ICodeSystemCode> GetCodeSystemCodesByValueSets(
            IEnumerable<Guid> valueSetGuids)
        {
            var guids = valueSetGuids as Guid[] ?? valueSetGuids.ToArray();
            if (!guids.Any())
            {
                return new List<ICodeSystemCode>();
            }

            var valueSetCodes = this.valueSetCodeService.GetValueSetCodes(guids)
                .GroupBy(vsc => vsc.CodeGuid)
                .Select(group => group.FirstOrDefault());

            return valueSetCodes.Select(vsc => vsc as ICodeSystemCode).Where(csc => csc != null);
        }

        private IEnumerable<ICodeSystemCode> GetCodeSystemCodes(
            IEnumerable<Guid> codeGuids)
        {
            var guids = codeGuids as Guid[] ?? codeGuids.ToArray();
            return guids.Any() ? this.codeSystemCodeService.GetCodeSystemCodes(guids) : new List<ICodeSystemCode>();
        }
    }
}