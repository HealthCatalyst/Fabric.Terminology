namespace Fabric.Terminology.API.Models
{
    using System.Collections.Generic;

    public class ValueSetApiModel : ValueSetMetaApiModel, IIdentifiable
    {
        public string Identifier { get; set; }

        public string ValueSetUniqueId { get; set; }

        public string ValueSetId { get; set; }

        public string ValueSetOId { get; set; }

        public string Name { get; set; }

        public bool IsCustom { get; set; }

        public bool AllCodesLoaded { get; set; }

        public int ValueSetCodesCount { get; set; }

        public IReadOnlyCollection<ValueSetCodeApiModel> ValueSetCodes { get; set; }
    }
}