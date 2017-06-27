namespace Fabric.Terminology.API.Models
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    public class ValueSetItem
    {
        public string AuthoringSourceDescription { get; set; }

        public string PurposeDescription { get; set; }

        public string SourceDescription { get; set; }

        public string VersionDescription { get; set; }

        public string ValueSetId { get; set; }

        public string Name { get; set; }

        public bool IsCustom { get; set; }

        public bool AllCodesLoaded { get; set; }

        public int ValueSetCodesCount { get; set; }

        public IReadOnlyCollection<ValueSetCodeItem> ValueSetCodes { get; set; }
    }
}