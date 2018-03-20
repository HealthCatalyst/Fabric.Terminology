namespace Fabric.Terminology.API.Models
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain;

    using Newtonsoft.Json;

    public class ValueSetFindByTermQuery : FindByTermQuery
    {
        [JsonProperty("summary")]
        public bool Summary { get; set; } = true;

        [JsonProperty("statusCodes")]
        public IEnumerable<ValueSetStatus> StatusCodes { get; set; } = new List<ValueSetStatus> { ValueSetStatus.Active };
    }
}