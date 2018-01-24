namespace Fabric.Terminology.API.Models
{
    using System;

    using Newtonsoft.Json;

    public class CompareValueSetsQuery : MultipleCodeSystemQuery
    {
        [JsonProperty("valueSetGuids")]
        public Guid[] ValueSetGuids { get; set; }
    }
}