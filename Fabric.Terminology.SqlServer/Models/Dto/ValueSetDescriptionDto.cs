// ReSharper disable InconsistentNaming

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fabric.Terminology.SqlServer.Models.Dto
{
    [Table("Terminology.ValueSetDescription")]
    internal sealed class ValueSetDescriptionDto
    {
        public string AuthoringSourceDSC { get; set; }

        public string BindingDSC { get; set; }

        //// [Key]
        public int BindingID { get; set; }

        //// [Key]
        public string BindingNM { get; set; }

        public string CreatedByNM { get; set; }

        public string DefinitionDSC { get; set; }

        //// [Key]
        public DateTime LastLoadDTS { get; set; }

        //// [Key]
        public string PublicFLG { get; set; }

        public string PurposeDSC { get; set; }

        //// [Column(TypeName = "datetime2")]
        public DateTime? RevisionDTS { get; set; }

        //// [Key]
        public string SourceDSC { get; set; }
        
        public string StatusCD { get; set; }

        public string TypeDSC { get; set; }

        //// [Key]
        public string ValueSetID { get; set; }

        public string ValueSetNM { get; set; }

        public string ValueSetOID { get; set; }

        //// [Key]
        public string VersionDSC { get; set; }
    }
}