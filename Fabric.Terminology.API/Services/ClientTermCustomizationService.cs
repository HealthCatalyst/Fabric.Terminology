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
                    parameters.CodeGuidsToAdd.ToList());
            }

            var invalidModel = new InvalidOperationException("Failed to validate ClientTermValueSetApiModel.  Possible duplicate codes detected between add and remove instructions.");
            return Attempt<IValueSet>.Failed(invalidModel);
        }

        public Attempt<IValueSet> UpdateValueSet(Guid valueSetGuid, ClientTermValueSetApiModel model)
        {
            var parameters = this.Build(model);
            if (parameters.IsValid())
            {
                return this.clientTermValueSetService.AddRemoveCodes(
                    Guid.NewGuid(),
                    parameters.CodeGuidsToAdd.ToList(),
                    parameters.CodeGuidsToRemove.ToList());
            }

            var invalidModel = new InvalidOperationException("Failed to validate ClientTermValueSetApiModel.  Possible duplicate codes detected between add and remove instructions.");
            return Attempt<IValueSet>.Failed(invalidModel);
        }

        private static IEnumerable<Guid> GetIdGuids(
            IEnumerable<CodeInstruction> codeInstructions,
            CodeSrc codeSrc,
            CodeInstructionType codeInstructionType)
        {
            return codeInstructions.Where(ci => ci.InstructionType == codeInstructionType && ci.Src == codeSrc)
                .Select(ci => ci.IdGuid);
        }

        private ClientTermParameters Build(ClientTermValueSetApiModel model)
        {
            var codesToAdd = this.GetCodesForInstruction(model, CodeInstructionType.Add);
            var codesToRemove = this.GetCodesForInstruction(model, CodeInstructionType.Remove);

            return new ClientTermParameters
            {
                Name = model.Name,
                ValueSetMeta = model,
                CodeGuidsToAdd = codesToAdd,
                CodeGuidsToRemove = codesToRemove
            };
        }

        private IEnumerable<ICodeSystemCode> GetCodesForInstruction(
            ClientTermValueSetApiModel model,
            CodeInstructionType codeInstructionType)
        {
            var codes = new List<ICodeSystemCode>();

            var valueSetGuids = GetIdGuids(model.CodeInstructions, CodeSrc.ValueSet, codeInstructionType);
            var codeGuids = GetIdGuids(model.CodeInstructions, CodeSrc.CodeSystemCode, codeInstructionType);

            codes.AddRange(this.GetCodeSystemCodesByValueSets(valueSetGuids));
            codes.AddRange(this.GetCodeSystemCodes(codeGuids, codeInstructionType));

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
            IEnumerable<Guid> codeGuids,
            CodeInstructionType codeInstructionType)
        {
            var guids = codeGuids as Guid[] ?? codeGuids.ToArray();
            return guids.Any() ? this.codeSystemCodeService.GetCodeSystemCodes(guids) : new List<ICodeSystemCode>();
        }

        private class ClientTermParameters
        {
            public string Name { get; set; }

            public IValueSetMeta ValueSetMeta { get; set; }

            public IEnumerable<ICodeSystemCode> CodeGuidsToAdd { get; set; } = new List<ICodeSystemCode>();

            public IEnumerable<ICodeSystemCode> CodeGuidsToRemove { get; set; } = new List<ICodeSystemCode>();

            public bool IsValid()
            {
                return !this.CodeGuidsToAdd.Select(cta => cta.CodeGuid)
                           .Any(ctag => this.CodeGuidsToRemove.Select(ctr => ctr.CodeGuid).Contains(ctag));
            }
        }
    }
}