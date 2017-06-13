using System.Collections.Generic;

namespace Fabric.Terminology.Domain.Models
{
    /// FYI this is just a quick view of the derived type and not intended at this point to be used.
    /// e.g. Making sure all properties are present and make sense.
    internal class ValueSet : IValueSet
    {
        public string ValueSetId { get; set; }
        public string Name { get; set; }
        public bool IsCustom { get; internal set; }
        public string AuthoringSourceDescription { get; internal set; }
        public string PurposeDescription { get; internal set; }
        public string SourceDescription { get; internal set; }
        public string VersionDescription { get; internal set; }
        public int ValueSetCodesCount { get; internal set; }
        public IEnumerable<IValueSetCode> ValueSetCodes { get; internal set; }
    }
}