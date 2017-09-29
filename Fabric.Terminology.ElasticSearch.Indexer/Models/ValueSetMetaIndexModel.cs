namespace Fabric.Terminology.ElasticSearch.Indexer.Models
{
	using System;

	public class ValueSetMetaIndexModel
    {
        public DateTime VersionDate { get; set; }

        public string DefinitionDescription { get; set; }

        public string AuthoringSourceDescription { get; set; }

        public string SourceDescription { get; set; }
    }
}