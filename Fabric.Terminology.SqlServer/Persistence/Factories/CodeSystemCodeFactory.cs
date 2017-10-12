namespace Fabric.Terminology.SqlServer.Persistence.Factories
{
    using System;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal class CodeSystemCodeFactory : IModelFactory<CodeSystemCodeDto, ICodeSystemCode>
    {
        public ICodeSystemCode Build(CodeSystemCodeDto dto)
        {
            if (dto.CodeSystem == null)
            {
                throw new NullReferenceException("CodeSystem property was null on CodeSystemCodeDto.  Did you forget to force the join?");
            }

            return new CodeSystemCode
            {
                CodeSystemGuid = dto.CodeSystemGUID,
                Code = dto.CodeCD,
                CodeGuid = dto.CodeGUID,
                CodeSystemName = dto.CodeSystem.CodeSystemNM,
                Retired = dto.Retired == "Y",
                Name = dto.CodeDSC
            };
        }
    }
}