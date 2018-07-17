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
            return this.Build(dto, false);
        }

        public IValueSetBackingItem Build(ValueSetDescriptionBaseDto dto, bool isCustom)
        {
            if (!Enum.TryParse(dto.StatusCD, true, out ValueSetStatus statusCode))
            {
                statusCode = ValueSetStatus.Active;
            }

            return new ValueSetBackingItem
            {
                AuthorityDescription = dto.AuthorityDSC.OrEmptyIfNull(),
                ClientCode = dto.ClientCD,
                DefinitionDescription = dto.DefinitionDSC.OrEmptyIfNull(),
                IsLatestVersion = dto.LatestVersionFLG == "Y",
                Name = dto.ValueSetNM,
                OriginGuid = dto.OriginGUID.GetValueOrDefault(),
                IsCustom = isCustom,
                SourceDescription = dto.SourceDSC.OrEmptyIfNull(),
                ValueSetGuid = dto.ValueSetGUID,
                ValueSetReferenceId = dto.ValueSetReferenceID,
                StatusCode = statusCode,
                VersionDate = dto.VersionDTS,
                LastModifiedDate = dto.LastModifiedDTS
            };
        }

        public IValueSetBackingItem Build(ValueSetDescriptionDto dto)
        {
            var item = this.Build(dto, dto.ClientTermFLG == "Y");
            return item;
        }
    }
}
