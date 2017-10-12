namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.SqlServer.Persistence.Factories;

    internal class ValueSet : Domain.Models.ValueSet
    {
        public ValueSet()
        {
        }

        public ValueSet(
            ValueSetDescriptionBASEDto descDto,
            IReadOnlyCollection<ValueSetCodeDto> codeDtos)
        {
            // TODO FixMe!!!
            var factory = new ValueSetCodeFactory();

            this.ValueSetGuid = descDto.ValueSetGUID;
            this.ValueSetReferenceId = descDto.ValueSetReferenceID;
            this.Name = descDto.ValueSetNM;
            this.VersionDate = descDto.VersionDTS;
            this.DefinitionDescription = descDto.DefinitionDSC;
            this.SourceDescription = descDto.SourceDSC;
            this.OriginGuid = descDto.OriginGUID.GetValueOrDefault();
            this.ClientCode = descDto.ClientCD;
            this.ValueSetCodes = codeDtos.Select(factory.Build).ToList();
        }
    }
}