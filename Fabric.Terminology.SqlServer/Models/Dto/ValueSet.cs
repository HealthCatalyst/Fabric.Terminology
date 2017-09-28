namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ValueSet : Domain.Models.ValueSet
    {
        public ValueSet()
        {
        }

        public ValueSet(
            ValueSetDescriptionDto descDto,
            IReadOnlyCollection<ValueSetCodeDto> codeDtos)
        {
            this.ValueSetGuid = descDto.ValueSetGUID;
            this.ValueSetReferenceId = descDto.ValueSetReferenceID;
            this.Name = descDto.ValueSetNM;
            this.VersionDate = descDto.VersionDTS;
            this.DefinitionDescription = descDto.DefinitionDSC;
            this.SourceDescription = descDto.SourceDSC;
            this.OriginGuid = descDto.OriginGUID;
            this.ClientCode = descDto.ClientCD;
            this.ValueSetCodes = codeDtos.Select(cd => new ValueSetCode(cd)).ToList();
        }
    }
}