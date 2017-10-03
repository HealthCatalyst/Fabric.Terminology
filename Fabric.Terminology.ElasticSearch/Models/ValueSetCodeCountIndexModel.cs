namespace Fabric.Terminology.ElasticSearch.Models
{
    using System;

    using Nest;

    [ElasticsearchType(Name = "valuesetcodecount")]
    public class ValueSetCodeCountIndexModel
    {
        [Keyword]
        public string ValueSetGuid { get; internal set; }

        [Keyword]
        public Guid CodeSystemGuid { get; internal set; }

        [Number]
        public int CodeCount { get; internal set; }
    }
}