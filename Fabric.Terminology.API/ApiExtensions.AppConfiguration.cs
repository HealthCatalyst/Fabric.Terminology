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
        internal static ClientCredentials CreateClientCredentials(
            this ClientSettings webConfigSettings,
            IdentityServerSettings appSettings,
            Uri identityServiceUri) =>
            new ClientCredentials
            {
                ClientId = string.IsNullOrWhiteSpace(webConfigSettings.ClientId) ? appSettings.ClientId : webConfigSettings.ClientId,
                ClientSecret = string.IsNullOrWhiteSpace(webConfigSettings.ClientSecret) ? appSettings.ClientSecret : webConfigSettings.ClientSecret,
                IdentityBaseUri = identityServiceUri
            };
    }
}
