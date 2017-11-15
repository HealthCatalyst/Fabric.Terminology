namespace Fabric.Terminology.API.Models
{
    using Fabric.Terminology.Domain;

    using Newtonsoft.Json;

    public class ValueSetFindByTermQuery : FindByTermQuery
    {
        [JsonProperty("summary")]
        public bool Summary { get; set; } = true;

        [JsonProperty("statusCode")]
        public ValueSetStatus StatusCode { get; set; } = ValueSetStatus.Active;
    }
}