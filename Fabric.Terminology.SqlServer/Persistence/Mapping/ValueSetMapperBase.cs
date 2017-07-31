namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal abstract class ValueSetMapperBase
    {
        protected IValueSet Build(ValueSetDescriptionDto dto, IReadOnlyCollection<IValueSetCode> codes, int codeCount)
        {
            return new ValueSet
            {
                ValueSetUniqueId = dto.ValueSetUniqueID,
                ValueSetId = dto.ValueSetID,
                ValueSetOId = dto.ValueSetOID,
                AuthoringSourceDescription = dto.AuthoringSourceDSC,
                Name = dto.ValueSetNM,
                IsCustom = false,
                PurposeDescription = dto.PurposeDSC,
                SourceDescription = dto.SourceDSC,
                VersionDescription = dto.VersionDSC,
                ValueSetCodes = codes,
                ValueSetCodesCount = codeCount
            };
        }
    }
}