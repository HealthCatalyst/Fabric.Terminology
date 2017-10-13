// ReSharper disable InconsistentNaming

namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    using Fabric.Terminology.Domain.Models;

    internal class ValueSetDescriptionBaseDto
    {
        private static readonly EmptySamdBinding EmptyBinding = new EmptySamdBinding();

        public ValueSetDescriptionBaseDto()
        {
        }

        public ValueSetDescriptionBaseDto(IValueSetBackingItem valueSet)
        {
            this.ValueSetGUID = valueSet.ValueSetGuid;
            this.ValueSetReferenceID = valueSet.ValueSetReferenceId;
            this.ValueSetNM = valueSet.Name;
            this.VersionDTS = valueSet.VersionDate;
            this.DefinitionDSC = valueSet.DefinitionDescription;
            this.SourceDSC = valueSet.SourceDescription;
            this.StatusCD = "Active";
            this.OriginGUID = valueSet.OriginGuid;
            this.ClientCD = valueSet.ClientCode;
            this.LastModifiedDTS = DateTime.UtcNow;
            this.LatestVersionFLG = "Y";
            this.BindingID = EmptyBinding.BindingID;
            this.BindingNM = EmptyBinding.BindingNM;
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

        public Guid? OriginGUID { get; set; }

        public string ClientCD { get; set; }

        public DateTime LastModifiedDTS { get; set; }

        public string LatestVersionFLG { get; set; }

        public int BindingID { get; set; }

        public string BindingNM { get; set; }

        public DateTime LastLoadDTS { get; set; }
    }
}