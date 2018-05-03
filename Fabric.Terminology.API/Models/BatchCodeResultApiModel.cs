namespace Fabric.Terminology.API.Models
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class BatchCodeResultApiModel
    {
        [JsonProperty("matches")]
        public IReadOnlyCollection<CodeSystemCodeApiModel> Matches { get; set; } = Array.Empty<CodeSystemCodeApiModel>();

        [JsonProperty("notFound")]
        public IReadOnlyCollection<string> NotFound { get; set; } = Array.Empty<string>();
    }
}
