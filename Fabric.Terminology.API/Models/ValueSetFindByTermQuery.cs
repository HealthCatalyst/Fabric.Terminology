namespace Fabric.Terminology.API.Models
{
    using Newtonsoft.Json;

    public class ValueSetFindByTermQuery : FindByTermQuery
    {
        [JsonProperty("summary")]
        public bool Summary { get; set; } = true;
    }
}