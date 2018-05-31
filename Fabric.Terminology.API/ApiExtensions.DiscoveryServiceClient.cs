namespace Fabric.Terminology.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Catalyst.DosApi.Discovery;
    using Catalyst.DosApi.Discovery.Catalyst.DiscoveryService.Models;
    using Catalyst.DosApi.Discovery.Configuration;

    using Fabric.Terminology.API.Configuration;

    /// <summary>
    /// Extensions methods related to <see cref="IDiscoveryServiceClient"/>
    /// </summary>
    public static partial class ApiExtensions
    {
        internal static Uri GetApiServiceForIdentityFromConfig(
            this IDiscoveryServiceClient discoveryServiceClient,
            IAppConfiguration config)
        {
            var task = RequestApiServiceForIdentityFromConfigAsync(discoveryServiceClient, config);
            task.Wait();
            return task.Result;
        }

        private static async Task<Uri> RequestApiServiceForIdentityFromConfigAsync(
            IDiscoveryServiceClient discoveryServiceClient,
            IAppConfiguration config)
        {
            var identityServiceName = config.IdentityServerSettings.ServiceNameInDiscovery;

            var configArgs = config.DiscoveryServiceClientSettings.DiscoveryServices.Where(
                a => a.ServiceName.Equals(identityServiceName, StringComparison.OrdinalIgnoreCase)).FirstMaybe();

            return await configArgs
                .Select(async args => await discoveryServiceClient.RequestServiceUriAsync(args.ServiceName, args.Version))
                .Else(() => throw new InvalidOperationException(FormattableString.Invariant($"Failed to find DiscoveryLookupArgs for '{identityServiceName}' in appsettings.json")));
        }
    }
}
