namespace Fabric.Terminology.API.Models
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class CompareValueSetsQuery : MultipleCodeSystemQuery
    {
        [JsonProperty("valueSetGuids")]
        public IEnumerable<Guid> ValueSetGuids { get; set; } = Array.Empty<Guid>();
    }
}