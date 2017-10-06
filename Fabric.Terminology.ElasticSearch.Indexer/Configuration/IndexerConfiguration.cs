namespace Fabric.Terminology.ElasticSearch.Indexer.Configuration
{
    using Fabric.Terminology.ElasticSearch.Configuration;
    using Fabric.Terminology.SqlServer.Configuration;

    public class IndexerConfiguration
    {
        public TerminologySqlSettings TerminologySqlSettings { get; set; }

        public ElasticSearchSettings ElasticSearchSettings { get; set; }
    }
}