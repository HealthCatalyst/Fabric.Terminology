// ReSharper disable InconsistentNaming
namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    internal sealed class ValueSetDescriptionDto
    {
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