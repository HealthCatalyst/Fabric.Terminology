namespace Fabric.Terminology.SqlServer.Persistence.Factories
{
    using System;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal sealed class ValueSetBackingItemFactory : IModelFactory<ValueSetDescriptionBaseDto, IValueSetBackingItem>
    {
        public IValueSetBackingItem Build(ValueSetDescriptionBaseDto dto)
        {
            if (!Enum.TryParse(dto.StatusCD, true, out ValueSetStatus statusCode))
            {
                statusCode = ValueSetStatus.Active;
            }

            return new ValueSetBackingItem
            {
                AuthoringSourceDescription = dto.AuthorityDSC,
                ClientCode = dto.ClientCD,
                DefinitionDescription = dto.DefinitionDSC,
                IsLatestVersion = dto.LatestVersionFLG == "Y",
                Name = dto.ValueSetNM,
                OriginGuid = dto.OriginGUID.GetValueOrDefault(),
                SourceDescription = dto.SourceDSC,
                ValueSetGuid = dto.ValueSetGUID,
                ValueSetReferenceId = dto.ValueSetReferenceID,
                StatusCode = statusCode,
                VersionDate = dto.VersionDTS
            };
        }

        public IValueSetBackingItem Build(ValueSetDescriptionDto dto)
        {
            var item = this.Build((ValueSetDescriptionBaseDto)dto);
            ((ValueSetBackingItem)item).IsCustom = dto.ClientTermFLG == "Y";
            return item;
        }
    }
}
