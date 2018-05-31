namespace Fabric.Terminology.API.Configuration
{
    using Catalyst.DosApi.Discovery.Configuration;

    using Fabric.Terminology.SqlServer.Configuration;

    public interface IAppConfiguration
    {
        /// <summary>
        /// Should be the URL of the terminology API without the version.
        /// </summary>
        string BaseTerminologyEndpoint { get; set; }

        TerminologySqlSettings TerminologySqlSettings { get; set; }

        HostingOptions HostingOptions { get; set; }

        IdentityServerSettings IdentityServerSettings { get; set; }

        DiscoveryRegistrationSettings DiscoveryRegistrationSettings { get; set; }

        DiscoveryServiceClientSettings DiscoveryServiceClientSettings { get; set; }

        ApplicationInsightsSettings ApplicationInsightsSettings { get; set; }
    }
}