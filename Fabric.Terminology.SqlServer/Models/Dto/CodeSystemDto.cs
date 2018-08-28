// ReSharper disable InconsistentNaming
namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    using JetBrains.Annotations;

    using NullGuard;

    public class CodeSystemDto
    {
        public Guid CodeSystemGUID { get; set; }

        public string CodeSystemNM { get; set; }

        public DateTime VersionDTS { get; set; }

        [AllowNull, CanBeNull]
        public string VersionDSC { get; set; }

        public string CodeSystemDSC { get; set; }

        [AllowNull, CanBeNull]
        public string CopyrightDSC { get; set; }

        [AllowNull, CanBeNull]
        public string OwnerDSC { get; set; }

        public int CodeCountNBR { get; set; }

        public DateTime LastModifiedDTS { get; set; }
    }
}
