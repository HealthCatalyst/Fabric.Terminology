namespace Fabric.Terminology.SqlServer.Configuration
{
    public class TerminologySqlSettings
    {
        public string ConnectionString { get; set; }

        public bool UseInMemory { get; set; } = false;
    }
}