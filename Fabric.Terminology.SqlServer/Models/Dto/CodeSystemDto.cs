// ReSharper disable InconsistentNaming
namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    public class CodeSystemDto
    {
        public Guid CodeSystemGuid { get; set; }

        public string CodeSystemNM { get; set; }

        public DateTime VersionDTS { get; set; }

        public string VersionDSC { get; set; }

        public string CodeSystemDSC { get; set; }

        public string CopyrightDSC { get; set; }

        public string OwnerDSC { get; set; }

        public int CodeCountNBR { get; set; }

        public DateTime LastModifiedDTS { get; set; }
    }
}
