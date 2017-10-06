namespace Fabric.Terminology.ElasticSearch.Configuration
{
    public class ElasticSearchSettings
    {
        public bool Enabled { get; set; }

        public bool UseSsl { get; set; }

        public string Hostname { get; set; }

        public string Port { get; set; }
    }
}
