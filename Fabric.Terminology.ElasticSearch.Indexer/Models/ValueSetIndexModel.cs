namespace Fabric.Terminology.ElasticSearch.Indexer.Models
{
	using System;
	using System.Collections.Generic;

	public class ValueSetIndexModel
    {
		public Guid Id { get; set; }
	    public Guid ValueSetGuid { get; set; }

	    public string ValueSetReferenceId { get; set; }

	    public string Name { get; set; }

		public string NameKeyword { get; set; }

	    public Guid OriginGuid { get; set; }

	    public string ClientCode { get; set; }

	    public DateTime VersionDate { get; set; }

	    public string DefinitionDescription { get; set; }

	    public string AuthoringSourceDescription { get; set; }

	    public string SourceDescription { get; set; }

		public bool IsCustom { get; set; }

	    public bool IsLatestVersion { get; set; }

	    public IReadOnlyCollection<ValueSetCodeCountIndexModel> CodeCounts { get; set; }

		public IReadOnlyCollection<ValueSetCodeIndexModel> ValueSetCodes { get; set; }
    }
}