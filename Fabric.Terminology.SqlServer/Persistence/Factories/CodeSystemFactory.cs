namespace Fabric.Terminology.SqlServer.Persistence.Factories
{
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal sealed class CodeSystemFactory : IModelFactory<CodeSystemDto, ICodeSystem>
    {
        public ICodeSystem Build(CodeSystemDto dto)
        {
            return new CodeSystem
            {
                CodeSystemGuid = dto.CodeSystemGUID,
                Copyright = dto.CopyrightDSC.OrEmptyIfNull(),
                Description = dto.CodeSystemDSC.OrEmptyIfNull(),
                Name = dto.CodeSystemNM.OrEmptyIfNull(),
                Owner = dto.OwnerDSC.OrEmptyIfNull(),
                CodeCount = dto.CodeCountNBR,
                VersionDate = dto.VersionDTS
            };
        }
    }
}
