// ReSharper disable InconsistentNaming

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fabric.Terminology.SqlServer.Models.Dto
{

    [Table("ValueSetCode", Schema = "Terminology")]
    internal sealed class ValueSetCodeDto
    {        
        //// [Key]
        public int BindingID { get; set; }

        //// [Key]
        public string BindingNM { get; set; }

        //// [Key]
        public string CodeCD { get; set; }

        public string CodeDSC { get; set; }

        //// [Key]
        public string CodeSystemCD { get; set; }

        public string CodeSystemNM { get; set; }

        public string CodeSystemVersionTXT { get; set; }

        //// [Key]
        public DateTime LastLoadDTS { get; set; }

        //// [Column(TypeName = "datetime2")]
        public DateTime? RevisionDTS { get; set; }

        //// [Key]
        public string SourceDSC { get; set; }

        //// [Key]
        public string ValueSetID { get; set; }

        public string ValueSetNM { get; set; }

        public string ValueSetOID { get; set; }

        //// [Key]
        public string VersionDSC { get; set; }
    }
}