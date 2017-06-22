using Fabric.Terminology.SqlServer.Configuration;

namespace Fabric.Terminology.API.Configuration
{
    public interface IAppConfiguration
    {
        TerminologySqlSettings TerminologySqlSettings { get; set; }
        ValueSetProxySettings ValueSetProxySettings { get; set; }
    }
}