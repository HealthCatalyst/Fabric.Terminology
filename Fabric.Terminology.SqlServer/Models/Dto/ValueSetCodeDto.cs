// ReSharper disable InconsistentNaming

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fabric.Terminology.SqlServer.Models.Dto
{
    [Table("Terminology.ValueSetCode")]
    internal sealed class ValueSetCodeDto
    {
        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BindingID { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(14)]
        public string BindingNM { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(100)]
        public string CodeCD { get; set; }

        [StringLength(4000)]
        public string CodeDSC { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(100)]
        public string CodeSystemCD { get; set; }

        [StringLength(1000)]
        public string CodeSystemNM { get; set; }

        [StringLength(1000)]
        public string CodeSystemVersionTXT { get; set; }

        [Key]
        [Column(Order = 7)]
        public DateTime LastLoadDTS { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? RevisionDTS { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string SourceDSC { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(255)]
        public string ValueSetID { get; set; }

        [StringLength(255)]
        public string ValueSetNM { get; set; }

        [StringLength(100)]
        public string ValueSetOID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string VersionDSC { get; set; }
    }
}