namespace Fabric.Terminology.API.Models
{
    using System;

    using Newtonsoft.Json;

    public class MultipleValueSetsQuery : MultipleCodeSystemQuery
    {
        [JsonProperty("valueSetGuids")]
        public Guid[] ValueSetGuids { get; set; }

        [JsonProperty("summary")]
        public bool Summary { get; set; } = true;
    }
}
