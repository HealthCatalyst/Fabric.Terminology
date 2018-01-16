using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fabric.Terminology.API.Services
{
    using CallMeMaybe;

    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;

    public class ClientTermValueSetApiService
    {
        private readonly ICodeSystemCodeService codeSystemCodeService;

        private readonly IValueSetCodeService valueSetCodeService;

        public ClientTermValueSetApiService(
            ICodeSystemCodeService codeSystemCodeService,
            IValueSetCodeService valueSetCodeService)
        {
            this.codeSystemCodeService = codeSystemCodeService;
            this.valueSetCodeService = valueSetCodeService;
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

        private IReadOnlyCollection<ICodeSystemCode> GetCodesForInstruction(
            ClientTermValueSetApiModel model,
            CodeInstructionType codeInstructionType)
        {
            var codes = new List<ICodeSystemCode>();

            var valueSetGuids = this.GetIdGuids(model.CodeInstructions, CodeSrc.ValueSet, codeInstructionType);
            var codeGuids = this.GetIdGuids(model.CodeInstructions, CodeSrc.CodeSystemCode, codeInstructionType);

            codes.AddRange(this.GetCodeSystemCodesByValueSets(valueSetGuids, codeInstructionType));
            codes.AddRange(this.GetCodeSystemCodes(codeGuids, codeInstructionType));

            return codes.GroupBy(csc => csc.CodeGuid)
                .Select(group => group.FirstOrDefault())
                .Where(csc => csc != null)
                .ToList();
        }

        private IEnumerable<Guid> GetIdGuids(
            IEnumerable<CodeInstruction> codeInstructions,
            CodeSrc codeSrc,
            CodeInstructionType codeInstructionType)
        {
            return codeInstructions.Where(ci => ci.InstructionType == codeInstructionType && ci.Src == codeSrc)
                .Select(ci => ci.IdGuid);
        }

        private IReadOnlyCollection<ICodeSystemCode> GetCodeSystemCodesByValueSets(
            IEnumerable<Guid> valueSetGuids,
            CodeInstructionType codeInstructionType)
        {
            var guids = valueSetGuids as Guid[] ?? valueSetGuids.ToArray();
            return guids.Any()
                       ? this.valueSetCodeService.GetValueSetCodes(guids)
                            .GroupBy(vsc => vsc.CodeGuid)
                            .Select(group => group.FirstOrDefault() as ICodeSystemCode)
                            .Where(csc => csc != null)
                            .ToList()
                       : new List<ICodeSystemCode>();
        }

        private IReadOnlyCollection<ICodeSystemCode> GetCodeSystemCodes(
            IEnumerable<Guid> codeGuids,
            CodeInstructionType codeInstructionType)
        {
            var guids = codeGuids as Guid[] ?? codeGuids.ToArray();
            return guids.Any() ?
                       this.codeSystemCodeService.GetCodeSystemCodes(guids) :
                       new List<ICodeSystemCode>();
        }

        private class ClientTermParameters
        {
            public string Name { get; set; }

            public IValueSetMeta ValueSetMeta { get; set; }

            public IEnumerable<ICodeSystemCode> CodeGuidsToAdd { get; set; } = new List<ICodeSystemCode>();

            public IEnumerable<ICodeSystemCode> CodeGuidsToRemove { get; set; } = new List<ICodeSystemCode>();

            public bool IsValid()
            {
                return !this.CodeGuidsToAdd
                    .Select(cta => cta.CodeGuid)
                    .Any(ctag => this.CodeGuidsToRemove.Select(ctr => ctr.CodeGuid).Contains(ctag));
            }
        }
    }
}
