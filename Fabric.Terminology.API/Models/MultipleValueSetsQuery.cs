namespace Fabric.Terminology.API.Models
{
    using Newtonsoft.Json;

    public class MultipleValueSetsQuery : CompareValueSetsQuery
    {
        [JsonProperty("summary")]
        public bool Summary { get; set; } = true;
    }
}
