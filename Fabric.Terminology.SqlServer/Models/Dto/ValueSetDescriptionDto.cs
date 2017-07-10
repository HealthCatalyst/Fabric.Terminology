// ReSharper disable InconsistentNaming

namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    internal sealed class ValueSetDescriptionDto
    {
        public string AuthoringSourceDSC { get; set; }

        public string BindingDSC { get; set; }

        public int BindingID { get; set; }

        public string BindingNM { get; set; }

        public string CreatedByNM { get; set; }

        public string DefinitionDSC { get; set; }

        public DateTime LastLoadDTS { get; set; }

        public string PublicFLG { get; set; }

        public string PurposeDSC { get; set; }

        public DateTime? RevisionDTS { get; set; }

        public string SourceDSC { get; set; }

        public string StatusCD { get; set; }

        public string TypeDSC { get; set; }

        public string ValueSetID { get; set; }

        public string ValueSetNM { get; set; }

        public string ValueSetOID { get; set; }

        public string VersionDSC { get; set; }
    }
}