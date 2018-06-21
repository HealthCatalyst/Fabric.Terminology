namespace Fabric.Terminology.API
{
    using System;
    using System.Security.Claims;

    using Catalyst.DosApi.Authorization.Compliance;

    using Fabric.Terminology.API.Infrastructure.PipelineHooks;
    using Fabric.Terminology.API.Services;

    using Nancy;
    using Nancy.Extensions;
    using Nancy.Security;

    /// <summary>
    /// Extension methods for <see cref="INancyModule"/>
    /// </summary>
    public static partial class ApiExtensions
    {
        internal static void RequiresAuthorizationPermission(
            this INancyModule module,
            UserAccessService userAccessService,
            PermissionName permissionName,
            params Predicate<Claim>[] requiredClaims)
        {
            module.AddBeforeHookOrExecute(SecurityHooks.RequiresAuthentication(), "Requires Authentication");
            module.AddBeforeHookOrExecute(SecurityHooks.RequiresClaims(requiredClaims), "Requires Claims");
            module.AddBeforeHookOrExecute(AuthorizationHooks.RequiresPermission(userAccessService, permissionName), "Requires Authorization Permission");
        }
    }
}
