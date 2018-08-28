namespace Fabric.Terminology.API.Services
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Catalyst.DosApi.Authorization;
    using Catalyst.DosApi.Authorization.Compliance;
    using Catalyst.DosApi.Authorization.Models;
    using Catalyst.Infrastructure.Caching;

    using Fabric.Terminology.API.Constants;
    using Fabric.Terminology.API.Infrastructure;

    using IdentityModel;

    using Nancy;

    public class UserAccessService
    {
        private readonly IUserPermissionsService userPermissionsService;

        private readonly IMemoryCacheProvider memoryCacheProvider;

        private readonly NancyContext context;

        public UserAccessService(
            NancyContextWrapper contextWrapper,
            IUserPermissionsService userPermissionsService,
            IMemoryCacheProvider memoryCacheProvider)
        {
            this.userPermissionsService = userPermissionsService;
            this.memoryCacheProvider = memoryCacheProvider;
            this.context = contextWrapper.Context;
        }

        /// <summary>
        /// Gets the Claims Identity Name
        /// </summary>
        /// <returns>The <see cref="ClaimsIdentity"/> Name</returns>
        /// <remarks>Ex. with Windows Auth: HQCATALYST\first.last</remarks>
        public Maybe<string> GetUsername() => this.context.CurrentUser?.Identity.Name;

        /// <summary>
        /// Gets the client id of the application from which the user token was created.
        /// </summary>
        /// <returns>The client id of the application that generated the user token</returns>
        /// <remarks>Ex. fabric-angularsample</remarks>
        public Maybe<string> GetClientId() => this.context.CurrentUser?.FindFirst(Claims.ClientId)?.Value;

        /// <summary>
        /// Gets the subject (unique in Authorization)
        /// </summary>
        /// <returns>The subject of the token</returns>
        /// <remarks>Ex. with Windows Auth: HQCATALYST\first.last</remarks>
        public Maybe<string> GetSubject() => this.context.CurrentUser?.FindFirst(JwtClaimTypes.Subject)?.Value;

        public Maybe<string> UserToken() =>
            this.context.Request.Headers[HttpHeaderValues.Authorization]
                .FirstMaybe()
                .Select(token => token.Substring(HttpHeaderValues.AuthorizationBearer.Length).Trim());

        public Task<bool> UserHasAccessAsync(PermissionName permissionName, int cacheDurationSeconds = 0) =>
            this.UserToken()
                .Select(
                    async token =>
                        {
                            var cacheKey = this.GetCacheKey(permissionName);

                            AccessPermissions access;
                            if (cacheDurationSeconds <= 0)
                            {
                                access = await this.userPermissionsService.GetPermissionsForUserViaDelegationAsync(token, permissionName.AsPermissionContext());
                            }
                            else
                            {
                                access = await this.memoryCacheProvider.GetItem<Task<AccessPermissions>>(
                                    cacheKey,
                                    () => this.userPermissionsService.GetPermissionsForUserViaDelegationAsync(
                                        token,
                                        permissionName.AsPermissionContext()),
                                    TimeSpan.FromSeconds(cacheDurationSeconds), // cache expiration
                                    false); // is sliding expiration
                            }

                            return !access.IsMissingRequiredPermission(permissionName);
                        })
                .Else(() => Task.FromResult(false));

        internal string GetCacheKey(PermissionName permissionName)
        {
            var subject = this.GetSubject()
                .Else(
                    () => throw new InvalidOperationException(
                              FormattableString.Invariant(
                                  $"Unable to create unique cache key without a value JWT subject")));

            return FormattableString.Invariant($"{typeof(AccessPermissions)}.{subject}.{permissionName}");
        }
    }
}