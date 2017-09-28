// ReSharper disable InconsistentNaming

namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    using Fabric.Terminology.Domain.Models;

    internal sealed class ValueSetCodeDto
    {
        public ValueSetCodeDto()
        {
        }

        public ValueSetCodeDto(IValueSetCode code)
        {
            // cody
            /*this.BindingNM = EmptyBinding.BindingNM,
            this.BindingID = EmptyBinding.BindingID,
            this.CodeCD = code.Code,
            this.CodeDSC = code.Name,
            this.CodeSystemNM = code.CodeSystem.Name,
            this.CodeSystemVersionTXT = code.CodeSystem.Version,
            this.CodeSystemCD = code.CodeSystem.Code,
            this.LastLoadDTS = code.LastLoadDate,
            this.SourceDSC = code.SourceDescription,
            this.ValueSetID = code.ValueSetId,
            this.ValueSetUniqueID = code.ValueSetUniqueId,
            this.ValueSetOID = code.ValueSetOId,
            this.ValueSetNM = code.ValueSetName,
            this.VersionDSC = code.VersionDescription*/
        }

        public Guid ValueSetGUID { get; set; }

        public Guid? CodeGUID { get; set; }

        public string CodeCD { get; set; }

        public string CodeDSC { get; set; }

        public Guid CodeSystemGuid { get; set; }

        public string CodeSystemNM { get; set; }

        public DateTime LastModifiedDTS { get; set; }

        public DateTime LastLoadDTS { get; set; }
    }
}