namespace Fabric.Terminology.ElasticSearch.Indexer.Models
{
	using System;

	public class CodeSetCodeIndexModel
    {
        public Guid CodeGuid { get; internal set; }

        public string Code { get; internal set; }

        public string Name { get; internal set; }

        public Guid CodeSystemGuid { get; internal set; }
    }
}