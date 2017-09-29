namespace Fabric.Terminology.ElasticSearch.Indexer.Models
{
	using System;

	public class ValueSetCodeCountIndexModel
    {
        public string ValueSetGuid { get; internal set; }

        public Guid CodeSystemGuid { get; internal set; }

        public int CodeCount { get; internal set; }
    }
}