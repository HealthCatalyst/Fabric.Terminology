namespace Fabric.Terminology.API.Models
{
    using System;

    using Newtonsoft.Json;

    public class MultipleCodeSystemQuery
    {
        [JsonProperty("codeSystemGuids")]
        public Guid[] CodeSystemGuids { get; set; } = Array.Empty<Guid>();
    }
}
