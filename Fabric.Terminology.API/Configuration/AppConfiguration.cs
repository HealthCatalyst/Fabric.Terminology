namespace Fabric.Terminology.API.Configuration
{
    using Catalyst.DosApi.Discovery.Configuration;

    using Fabric.Terminology.SqlServer.Configuration;

    using NullGuard;

    public class AppConfiguration : IAppConfiguration
    {
        [AllowNull]
        public TerminologySqlSettings TerminologySqlSettings { get; set; }

        [AllowNull]
        public HostingOptions HostingOptions { get; set; }

        public IdentityServerConfidentialClientSettings IdentityServerSettings { get; set; }

        public DiscoveryServiceClientSettings DiscoveryServiceClientSettings { get; set; }

        public ApplicationInsightsSettings ApplicationInsightsSettings { get; set; }
    }
}