namespace Fabric.Terminology.ElasticSearch.Indexer.Models
{
	using System;
	using System.Collections.Generic;

	public class ValueSetIndexModel : ValueSetItemIndexModel
    {
		public Guid Id { get; set; }

        public IReadOnlyCollection<ValueSetCodeIndexModel> ValueSetCodes { get; set; }
    }
}