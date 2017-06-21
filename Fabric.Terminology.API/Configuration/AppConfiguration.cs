using Fabric.Terminology.SqlServer.Configuration;

namespace Fabric.Terminology.API.Configuration
{
    public class AppConfiguration : IAppConfiguration
    {
        public TerminologySqlSettings TerminologySqlSettings { get; set; }
    }
}