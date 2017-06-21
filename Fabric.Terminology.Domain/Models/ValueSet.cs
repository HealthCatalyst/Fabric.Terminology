using System.Collections.Generic;

namespace Fabric.Terminology.Domain.Models
{
    public class ValueSet : IValueSet
    {
        public string ValueSetId { get; set; }
        public string Name { get; set; }
        public bool IsCustom { get; internal set; }
        public string AuthoringSourceDescription { get; internal set; }
        public string PurposeDescription { get; internal set; }
        public string SourceDescription { get; internal set; }
        public string VersionDescription { get; internal set; }
        public int ValueSetCodesCount { get; internal set; }
        public IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; set; }
    }
}