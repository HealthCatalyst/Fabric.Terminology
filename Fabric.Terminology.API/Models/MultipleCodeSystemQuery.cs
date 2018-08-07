namespace Fabric.Terminology.API.Models
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class MultipleCodeSystemQuery
    {
        [JsonProperty("codeSystemGuids")]
        public IEnumerable<Guid> CodeSystemGuids { get; set; } = Array.Empty<Guid>();
    }
}
