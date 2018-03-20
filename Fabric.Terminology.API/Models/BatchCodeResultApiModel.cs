namespace Fabric.Terminology.API.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class BatchCodeResultApiModel
    {
        [JsonProperty("matches")]
        public IReadOnlyCollection<CodeSystemCodeApiModel> Matches { get; set; }

        [JsonProperty("notFound")]
        public IReadOnlyCollection<string> NotFound { get; set; }
    }
}
