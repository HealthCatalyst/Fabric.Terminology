namespace Fabric.Terminology.SqlServer.Persistence.Factories
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal sealed class ValueSetBackingItemFactory : IModelFactory<ValueSetDescriptionDto, IValueSetBackingItem>
    {
        public IValueSetBackingItem Build(ValueSetDescriptionDto dto)
        {
            return new ValueSetBackingItem
            {
                AuthoringSourceDescription = dto.AuthorityDSC,
                ClientCode = dto.ClientCD,
                DefinitionDescription = dto.DefinitionDSC,
                IsCustom = dto.ClientTermFLG == "Y",
                Name = dto.ValueSetNM,
                OriginGuid = dto.OriginGUID,
                SourceDescription = dto.SourceDSC,
                ValueSetGuid = dto.ValueSetGUID,
                ValueSetReferenceId = dto.ValueSetReferenceID,
                VersionDate = dto.VersionDTS
            };
        }
    }
}
