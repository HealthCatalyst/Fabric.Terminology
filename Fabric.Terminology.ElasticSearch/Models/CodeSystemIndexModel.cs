namespace Fabric.Terminology.ElasticSearch.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    using Nest;

    [ElasticsearchType(Name = "codesystem")]
    public class CodeSystemIndexModel : ICodeSystem, IIndexModel
    {
        [Text]
        public string Id { get; set; }

        [Keyword]
        public Guid CodeSystemGuid { get; set; }

        [Text]
        public string Name { get; set; }

        [Date(Format = "yyyy-MM-dd'T'HH:mm:ss")]
        public DateTime VersionDate { get; set; }

        [Text]
        public string Description { get; set; }

        [Text]
        public string Copyright { get; set; }

        [Keyword]
        public string Owner { get; set; }

        public int CodeCount { get; set; }
    }
}