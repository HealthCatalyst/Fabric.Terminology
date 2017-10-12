// ReSharper disable InconsistentNaming
namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    public class CodeSystemCodeDto
    {
        public Guid CodeGUID { get; set; }

        public string CodeCD { get; set; }

        public string CodeDSC { get; set; }

        public Guid CodeSystemGUID { get; set; }

        public DateTime LastModifiedDTS { get; set; }

        public DateTime LastLoadDTS { get; set; }

        public CodeSystemDto CodeSystem { get; set; }
    }
}