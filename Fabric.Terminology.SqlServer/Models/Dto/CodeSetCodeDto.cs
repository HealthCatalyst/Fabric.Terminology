// ReSharper disable InconsistentNaming

namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    public class CodeSetCodeDto
    {
        public int BindingID { get; set; }

        public string BindingNM { get; set; }

        public string CodeCD { get; set; }

        public string CodeDSC { get; set; }

        public string CodeSystemCD { get; set; }

        public string CodeSystemNM { get; set; }

        public string CodeSystemVersionTXT { get; set; }

        public DateTime LastLoadDTS { get; set; }

        public DateTime? RevisionDTS { get; set; }

        public string SourceDSC { get; set; }

        public string ValueSetUniqueID { get; set; }

        public string ValueSetID { get; set; }

        public string ValueSetNM { get; set; }

        public string ValueSetOID { get; set; }

        public string VersionDSC { get; set; }
    }
}