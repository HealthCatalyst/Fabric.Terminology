namespace Fabric.Terminology.SqlServer.Persistence.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal class CodeSystemCodeFactory : IModelFactory<CodeSystemCodeDto, ICodeSystemCode>
    {
        private readonly IReadOnlyCollection<ICodeSystem> codeSystems;

        public CodeSystemCodeFactory(IReadOnlyCollection<ICodeSystem> codeSystems)
        {
            this.codeSystems = codeSystems;
        }

        public ICodeSystemCode Build(CodeSystemCodeDto dto)
        {
            return new CodeSystemCode
            {
                CodeSystemGuid = dto.CodeSystemGUID,
                Code = dto.CodeCD,
                CodeGuid = dto.CodeGUID,
                CodeSystemName = this.GetCodeSystemName(dto),
                Retired = dto.RetiredFLG == "Y",
                Name = dto.CodeDSC.OrEmptyIfNull()
            };
        }

        private string GetCodeSystemName(CodeSystemCodeDto dto) =>
            Maybe.From(this.codeSystems.FirstOrDefault(cs => cs.CodeSystemGuid == dto.CodeSystemGUID))
                .Select(cs => cs.Name)
                .Else(() => string.Empty);
    }
}