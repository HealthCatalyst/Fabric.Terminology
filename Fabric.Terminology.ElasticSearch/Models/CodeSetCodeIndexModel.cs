namespace Fabric.Terminology.ElasticSearch.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    using Nest;

    [ElasticsearchType(Name = "codesetcode")]
    public class CodeSetCodeIndexModel : ICodeSetCode
    {
        [Keyword]
        public Guid CodeGuid { get; internal set; }

        [Keyword]
        public string Code { get; internal set; }

        [Text]
        public string Name { get; internal set; }

        [Keyword]
        public Guid CodeSystemGuid { get; set; }

        [Text]
        public string CodeSystemName { get; set; }
    }
}