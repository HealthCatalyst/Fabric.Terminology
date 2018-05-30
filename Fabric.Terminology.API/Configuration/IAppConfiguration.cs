namespace Fabric.Terminology.API.Configuration
{
    using Catalyst.DosApi.Discovery.Configuration;

    using Fabric.Terminology.SqlServer.Configuration;

    public interface IAppConfiguration
    {
        TerminologySqlSettings TerminologySqlSettings { get; set; }

        HostingOptions HostingOptions { get; set; }

        IdentityServerConfidentialClientSettings IdentityServerSettings { get; set; }

        DiscoveryServiceClientSettings DiscoveryServiceClientSettings { get; set; }

        ApplicationInsightsSettings ApplicationInsightsSettings { get; set; }
    }
}