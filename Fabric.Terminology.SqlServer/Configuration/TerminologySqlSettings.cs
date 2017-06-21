namespace Fabric.Terminology.SqlServer.Configuration
{
    public class TerminologySqlSettings
    {
        public string ConnectionString { get; set; }

        public bool UseInMemory { get; set; } = false;

        public bool MemoryCacheEnabled { get; set; } = true;

        public bool MemoryCacheSliding { get; set; } = true;

        public int MemoryCacheMinDuration { get; set; } = 5;

        public int DefaultItemsPerPage { get; set; } = 500;

        public bool LogGeneratedSql { get; set; } = false;
    }
}