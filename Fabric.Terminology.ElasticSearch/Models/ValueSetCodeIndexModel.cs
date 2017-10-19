namespace Fabric.Terminology.ElasticSearch.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    using Nest;

    [ElasticsearchType(Name = "valuesetcode")]
    public class ValueSetCodeIndexModel : CodeSystemCodeIndexModel, IValueSetCode
    {
        [Keyword]
        public Guid ValueSetGuid { get; set; }
    }
}