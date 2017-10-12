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
                Copyright = dto.CopyrightDSC.IsNullOrWhiteSpace() ? string.Empty : dto.CopyrightDSC,
                Description = dto.CodeSystemDSC.IsNullOrWhiteSpace() ? string.Empty : dto.CodeSystemDSC,
                Name = dto.CodeSystemNM,
                Owner = dto.OwnerDSC.IsNullOrWhiteSpace() ? string.Empty : dto.OwnerDSC,
                CodeCount = dto.CodeCountNBR,
                VersionDate = dto.VersionDTS
            };
        }
    }
}
