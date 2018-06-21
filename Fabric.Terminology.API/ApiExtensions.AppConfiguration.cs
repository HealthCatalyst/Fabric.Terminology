namespace Fabric.Terminology.API
{
    using System;

    using Catalyst.DosApi.Identity.Models;

    using Fabric.Terminology.API.Configuration;

    /// <summary>
    /// Extension methods related to application configuration
    /// </summary>
    public static partial class ApiExtensions
    {
        internal static ClientCredentials CreateClientCredentials(this IdentityServerSettings settings, Uri identityServiceUri) =>
            new ClientCredentials
            {
                ClientId = settings.ClientId,
                ClientSecret = settings.ClientSecret,
                IdentityBaseUri = identityServiceUri
            };
    }
}
