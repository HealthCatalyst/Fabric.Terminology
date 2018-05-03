namespace Fabric.Terminology.API.Models
{
    using System;

    using Newtonsoft.Json;

    internal class BatchCodeQuery : MultipleCodeSystemQuery
    {
        [JsonProperty("codes")]
        public string[] Codes { get; set; } = Array.Empty<string>();
    }
}