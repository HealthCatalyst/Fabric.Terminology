namespace Fabric.Terminology.ElasticSearch.Indexer.Models
{
	using System;

	using global::Nest;

	public class ValueSetCodeIndexModel : CodeSetCodeIndexModel
    {
        public Guid ValueSetGuid { get; set; }
    }
}