namespace Fabric.Terminology.API.Models
{
    using Newtonsoft.Json;

    internal class BatchCodeQuery : MultipleCodeSystemQuery
    {
        [JsonProperty("codes")]
        public string[] Codes { get; set; }
    }
}