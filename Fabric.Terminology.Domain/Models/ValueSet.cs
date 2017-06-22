using System.Collections.Generic;

namespace Fabric.Terminology.Domain.Models
{
    public class ValueSet : IValueSet
    {
        public string ValueSetId { get; set; }
        public string Name { get; set; }
        public bool IsCustom { get; set; }
        public string AuthoringSourceDescription { get; set; }
        public string PurposeDescription { get; set; }
        public string SourceDescription { get; set; }
        public string VersionDescription { get;set; }
        public bool AllCodesLoaded => ValueSetCodesCount == ValueSetCodes.Count;

        /// <summary>
        /// Gets or sets the number (count) of codes in the value set.
        /// </summary>
        /// <remarks>
        /// Value is set, rather than calculated, so that API can return a summary object with the first X number of codes leaving 
        /// this count correct for representation in the UI.
        /// </remarks>
        public int ValueSetCodesCount { get; set; }
        public IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; set; } = new IValueSetCode[]{};        
    }
}