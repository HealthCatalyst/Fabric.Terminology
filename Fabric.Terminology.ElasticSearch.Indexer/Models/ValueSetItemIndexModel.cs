namespace Fabric.Terminology.ElasticSearch.Indexer.Models
{
	using System;
	using System.Collections.Generic;

	public class ValueSetItemIndexModel : ValueSetMetaIndexModel
    {
        public Guid ValueSetGuid { get; set; }

        public string ValueSetReferenceId { get; set; }

        public string Name { get; set; }

        public Guid OriginGuid { get; set; }

        public string ClientCode { get; set; }

        public bool IsCustom { get; set; }

        public bool IsLatestVersion { get; set; }

        public IReadOnlyCollection<ValueSetCodeCountIndexModel> CodeCounts { get; set; }
    }
}