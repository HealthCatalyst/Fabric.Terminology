namespace Fabric.Terminology.ElasticSearch.Models
{
    using System;

    using Nest;

    [ElasticsearchType(Name = "codesetcode")]
    public class CodeSetCodeIndexModel
    {
        [Keyword]
        public Guid CodeGuid { get; internal set; }

        [Keyword]
        public string Code { get; internal set; }

        [Text]
        public string Name { get; internal set; }

        [Keyword]
        public Guid CodeSystemGuid { get; internal set; }
    }
}