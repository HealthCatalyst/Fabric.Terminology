// ReSharper disable InconsistentNaming
namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    internal sealed class ValueSetCodeDto
    {
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