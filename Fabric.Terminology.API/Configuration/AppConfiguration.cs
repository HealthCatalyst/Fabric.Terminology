namespace Fabric.Terminology.API.Configuration
{
    using Fabric.Terminology.ElasticSearch.Configuration;
    using Fabric.Terminology.SqlServer.Configuration;

    public class AppConfiguration : IAppConfiguration
    {
        public TerminologySqlSettings TerminologySqlSettings { get; set; }

        public ElasticSearchSettings ElasticSearchSettings { get; set; }

        public HostingOptions HostingOptions { get; set; }
    }
}