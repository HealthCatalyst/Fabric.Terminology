// ReSharper disable InconsistentNaming

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fabric.Terminology.SqlServer.Models.Dto
{
    [Table("Terminology.ValueSetDescription")]
    internal sealed class ValueSetDescriptionDto
    {
        [StringLength(1000)]
        public string AuthoringSourceDSC { get; set; }

        [StringLength(1000)]
        public string BindingDSC { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BindingID { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(21)]
        public string BindingNM { get; set; }

        [StringLength(255)]
        public string CreatedByNM { get; set; }

        public string DefinitionDSC { get; set; }

        [Key]
        [Column(Order = 6)]
        public DateTime LastLoadDTS { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(255)]
        public string PublicFLG { get; set; }

        public string PurposeDSC { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? RevisionDTS { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(255)]
        public string SourceDSC { get; set; }

        [StringLength(1000)]
        public string StatusCD { get; set; }

        [StringLength(1000)]
        public string TypeDSC { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(255)]
        public string ValueSetID { get; set; }

        [StringLength(1000)]
        public string ValueSetNM { get; set; }

        [StringLength(255)]
        public string ValueSetOID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(255)]
        public string VersionDSC { get; set; }
    }
}