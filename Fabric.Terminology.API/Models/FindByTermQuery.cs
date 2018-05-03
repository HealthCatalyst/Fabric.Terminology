namespace Fabric.Terminology.API.Models
{
    using Fabric.Terminology.Domain.Models;

    using Newtonsoft.Json;

    public class FindByTermQuery : MultipleCodeSystemQuery
    {
        [JsonProperty("term")]
        public string Term { get; set; } = string.Empty;

        [JsonProperty("pagerSettings")]
        public PagerSettings PagerSettings { get; set; } = new PagerSettings();
    }
}