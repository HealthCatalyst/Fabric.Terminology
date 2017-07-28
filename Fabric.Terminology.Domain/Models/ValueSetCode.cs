namespace Fabric.Terminology.Domain.Models
{
    public class ValueSetCode : CodeSetCode, IValueSetCode
    {
        public string ValueSetUniqueId { get; set; }

        public string ValueSetId { get; set; }
    }
}