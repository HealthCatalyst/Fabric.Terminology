namespace Fabric.Terminology.ElasticSearch.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    using Nest;

    [ElasticsearchType(Name = "valuesetcodecount")]
    public class ValueSetCodeCountIndexModel : IValueSetCodeCount
    {
        [Keyword]
        public Guid ValueSetGuid { get; set; }

        [Keyword]
        public Guid CodeSystemGuid { get; set; }

        [Number]
        public int CodeCount { get; set; }
    }
}