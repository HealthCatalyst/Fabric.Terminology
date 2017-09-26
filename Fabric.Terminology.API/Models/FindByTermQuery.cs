namespace Fabric.Terminology.API.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    using Newtonsoft.Json;

    public class FindByTermQuery
    {
        [JsonProperty("term")]
        public string Term { get; set; }

        [JsonProperty("pagerSettings")]
        public PagerSettings PagerSettings { get; set; }

        [JsonProperty("codeSystemGuids")]
        public Guid[] CodeSystemGuids { get; set; }

        [JsonProperty("summary")]
        public bool Summary { get; set; } = true;
    }
}