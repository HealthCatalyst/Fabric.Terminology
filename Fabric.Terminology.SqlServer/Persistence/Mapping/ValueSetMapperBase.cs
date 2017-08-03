namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Strategy;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal abstract class ValueSetMapperBase
    {
        private readonly IIsCustomValueStrategy isCustomValue;

        protected ValueSetMapperBase(IIsCustomValueStrategy isCustomValueStrategy)
        {
            this.isCustomValue = isCustomValueStrategy;
        }

        protected IValueSet Build(ValueSetDescriptionDto dto, IReadOnlyCollection<IValueSetCode> codes, int codeCount)
        {
            var valueSet = new ValueSet(
                dto.ValueSetID,
                dto.ValueSetUniqueID,
                dto.ValueSetOID,
                dto.ValueSetNM,
                dto.AuthoringSourceDSC,
                dto.PurposeDSC,
                dto.VersionDSC,
                codes)
            {
                ValueSetCodesCount = codeCount                
            };

            this.isCustomValue.Set(valueSet);

            return valueSet;
        }
    }
}