namespace Fabric.Terminology.ElasticSearch.Models
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    using Nest;

    [ElasticsearchType(Name = "valueset")]
    public class ValueSetIndexModel : IValueSetBackingItem
    {
        [Text]
        public string Id { get; set; }

        [Keyword]
        public Guid ValueSetGuid { get; set; }

        [Keyword]
        public string ValueSetReferenceId { get; set; }

        public string Name { get; set; }

        [Keyword]
        public Guid OriginGuid { get; set; }

        [Keyword]
        public string ClientCode { get; set; }

        [Date(Format = "yyyy-MM-dd'T'HH:mm:ss")]
        public DateTime VersionDate { get; set; }

        [Text]
        public string DefinitionDescription { get; set; }

        [Text]
        public string AuthoringSourceDescription { get; set; }

        [Text]
        public string SourceDescription { get; set; }

        [Boolean(NullValue = false, Store = false)]
        public bool IsCustom { get; set; }

        [Boolean(NullValue = true, Store = true)]
        public bool IsLatestVersion { get; set; }

        [Nested]
        public IReadOnlyCollection<ValueSetCodeCountIndexModel> CodeCounts { get; set; }

        [Nested]
        public IReadOnlyCollection<ValueSetCodeIndexModel> ValueSetCodes { get; set; }
    }
}