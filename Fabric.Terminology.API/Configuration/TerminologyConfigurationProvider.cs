namespace Fabric.Terminology.API.Configuration
{
    using System.Collections.Generic;

    using Catalyst.DosApi.Discovery.Configuration;

    using Microsoft.Extensions.Configuration;

    public class TerminologyConfigurationProvider
    {
        public IAppConfiguration GetAppConfiguration(IConfiguration configuration)
        {
            var appConfig = new AppConfiguration();
            configuration.Bind(appConfig);

            appConfig.DiscoveryRegistrationSettings.ServiceName = TerminologyVersion.DiscoveryServiceName;

            // Manually deserialize the list of DiscoveryLookupArgs
            var discoveryServices = configuration.GetSection("DiscoveryServiceClientSettings:DiscoveryServices")
                .Get<Dictionary<string, DiscoveryLookupArgs>>();
            appConfig.DiscoveryServiceClientSettings.DiscoveryServices = discoveryServices;

            return appConfig;
        }
    }
}