namespace Fabric.Terminology.ElasticSearch.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    using Nest;

    [ElasticsearchType(Name = "valuesetcode")]
    public class ValueSetCodeIndexModel : CodeSetCodeIndexModel, IValueSetCode
    {
        [Keyword]
        public Guid ValueSetGuid { get; set; }
    }
}