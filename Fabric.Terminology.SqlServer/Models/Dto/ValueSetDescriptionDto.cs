// ReSharper disable InconsistentNaming

namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    using Fabric.Terminology.Domain.Models;

    internal sealed class ValueSetDescriptionDto
    {
        private static readonly EmptySamdBinding EmptyBinding = new EmptySamdBinding();

        public ValueSetDescriptionDto()
        {
        }

        public ValueSetDescriptionDto(IValueSet valueSet)
        {
            this.ValueSetGUID = valueSet.ValueSetGuid;
            this.ValueSetReferenceID = valueSet.ValueSetReferenceId;
            this.ValueSetNM = valueSet.Name;
            this.VersionDTS = valueSet.VersionDate;
            this.DefinitionDSC = valueSet.DefinitionDescription;
            this.AuthorityDSC = string.Empty; // cody?
            this.SourceDSC = valueSet.SourceDescription;
            this.StatusCD = "Active";
            this.OriginGUID = valueSet.OriginGuid;
            this.ClientCD = valueSet.ClientCode;
            this.LastModifiedDTS = default(DateTime); // cody?
            this.ClientTermFLG = string.Empty; // cody?
            this.LatestVersionFLG = "Y";
            this.LastLoadDTS = EmptyBinding.LastLoadDts;
        }

        public Guid ValueSetGUID { get; set; }

        public string ValueSetReferenceID { get; set; }

        public string ValueSetNM { get; set; }

        public DateTime VersionDTS { get; set; }

        public string DefinitionDSC { get; set; }

        public string AuthorityDSC { get; set; }

        public string SourceDSC { get; set; }

        public string StatusCD { get; set; }

        public Guid OriginGUID { get; set; }

        public string ClientCD { get; set; }

        public DateTime LastModifiedDTS { get; set; }

        public string ClientTermFLG { get; set; }

        public string LatestVersionFLG { get; set; }

        public DateTime LastLoadDTS { get; set; }
    }
}