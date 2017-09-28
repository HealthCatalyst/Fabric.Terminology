// ReSharper disable InconsistentNaming

namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    using Fabric.Terminology.Domain.Models;

    internal sealed class ValueSetDescriptionDto
    {
        public ValueSetDescriptionDto()
        {
        }

        public ValueSetDescriptionDto(IValueSet valueSet)
        {
            //cody
            //return new ValueSetDescriptionDto
            //{
            //    AuthoringSourceDSC = valueSet.AuthoringSourceDescription,
            //    BindingID = EmptyBinding.BindingID,
            //    BindingNM = EmptyBinding.BindingNM,
            //    CreatedByNM = Constants.ValueSetDescriptionDto.CreatedByNM,
            //    DefinitionDSC = string.Empty,
            //    VersionDSC = valueSet.VersionDescription,
            //    LastLoadDTS = EmptyBinding.GetLastLoadDTS(),
            //    PublicFLG = "Y",
            //    LatestVersionFLG = "Y",
            //    StatusCD = "Active",
            //    PurposeDSC = valueSet.DefinitionDescription,
            //    RevisionDTS = null,
            //    SourceDSC = valueSet.SourceDescription,
            //    ValueSetUniqueID = valueSet.ValueSetUniqueId,
            //    ValueSetID = valueSet.ValueSetId,
            //    ValueSetOID = valueSet.ValueSetOId,
            //    ValueSetNM = valueSet.Name
            //};
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