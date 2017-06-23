namespace Fabric.Terminology.API.Configuration
{
    using Fabric.Terminology.SqlServer.Configuration;

    public interface IAppConfiguration
    {
        TerminologySqlSettings TerminologySqlSettings { get; set; }

        ValueSetProxySettings ValueSetProxySettings { get; set; }
    }
}