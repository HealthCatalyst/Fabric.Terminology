namespace Fabric.Terminology.ElasticSearch.Indexer.Models
{
	using System;

	public class ValueSetCodeIndexModel : CodeSetCodeIndexModel
    {
        public Guid ValueSetGuid { get; set; }
    }
}