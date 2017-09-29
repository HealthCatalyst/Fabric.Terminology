namespace Fabric.Terminology.ElasticSearch.Indexer.Nest
{
	using System;

	using global::Nest;

    // This is to test the proof of concept.  Should be load via configuration.
	public class ElasticConnectionFactory
	{
		public ElasticClient Create() => new ElasticClient(this.GetConnectionSettings());

		private static Uri CreateUri(int port) => new Uri("http://localhost:" + port);

		private ConnectionSettings GetConnectionSettings() => new ConnectionSettings(CreateUri(9200));
	}
}
