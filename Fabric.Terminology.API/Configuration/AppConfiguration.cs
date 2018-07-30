namespace Fabric.Terminology.API.Configuration
{
    using Catalyst.DosApi.Discovery.Configuration;

    using Fabric.Terminology.SqlServer.Configuration;

    using NullGuard;

    public class AppConfiguration : IAppConfiguration
    {
        [AllowNull]
        public string BaseTerminologyEndpoint { get; set; }

        [AllowNull]
        public string SwaggerRootBasePath { get; set; }

        [AllowNull]
        public TerminologySqlSettings TerminologySqlSettings { get; set; }

        [AllowNull]
        public HostingOptions HostingOptions { get; set; }

        [AllowNull]
        public IdentityServerSettings IdentityServerSettings { get; set; }

        [AllowNull]
        public DiscoveryRegistrationSettings DiscoveryRegistrationSettings { get; set; }

        [AllowNull]
        public DiscoveryServiceClientSettings DiscoveryServiceClientSettings { get; set; }

        [AllowNull]
        public ApplicationInsightsSettings ApplicationInsightsSettings { get; set; }
    }
}