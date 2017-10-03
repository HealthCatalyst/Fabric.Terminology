namespace Fabric.Terminology.ElasticSearch.Models
{
    using System;

    using Nest;

    [ElasticsearchType(Name = "valuesetcode")]
    public class ValueSetCodeIndexModel : CodeSetCodeIndexModel
    {
        [Keyword]
        public Guid ValueSetGuid { get; set; }
    }
}