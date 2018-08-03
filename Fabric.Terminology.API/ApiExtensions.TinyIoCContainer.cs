namespace Fabric.Terminology.API
{
    using System;

    using Catalyst.DosApi.Authorization;
    using Catalyst.DosApi.Common;
    using Catalyst.DosApi.Identity.Models;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.Domain.DependencyInjection;

    using Nancy.TinyIoc;

    /// <summary>
    /// Extension methods for Nancy's <see cref="TinyIoCContainer"/>
    /// </summary>
    public static partial class ApiExtensions
    {
        internal static TinyIoCContainer ComposeFrom<TComposition>(this TinyIoCContainer container)
            where TComposition : class, IContainerComposition<TinyIoCContainer>, new()
        {
            var composition = new TComposition();
            composition.Compose(container);
            return container;
        }

        internal static TinyIoCContainer RegisterDosServices(
            this TinyIoCContainer container,
            IdentityServerSettings settings,
            Uri identityServiceUri,
            Uri authorizationServiceUri)
        {
            container.Register<ClientCredentials>(settings.CreateClientCredentials(identityServiceUri));
            container.Register<FabricServiceLocations>((c, p) => new FabricServiceLocations(identityServiceUri, authorizationServiceUri));
            container.Register<IAuthenticatedApiClientFactory, AuthenticatedApiClientFactory>().AsSingleton();
            container.Register<IUserPermissionsService>(
                (c, p) => new UserPermissionsService(
                    c.Resolve<IAuthenticatedApiClientFactory>(),
                    c.Resolve<FabricServiceLocations>().AuthorizationServiceUri));

            return container;
        }
    }
}