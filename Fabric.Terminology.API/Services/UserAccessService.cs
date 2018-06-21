namespace Fabric.Terminology.API.Services
{
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Catalyst.DosApi.Authorization;
    using Catalyst.DosApi.Authorization.Compliance;
    using Catalyst.DosApi.Authorization.Models;
    using Catalyst.DosApi.Authorization.Models.Dtos;

    using Fabric.Terminology.API.Constants;
    using Fabric.Terminology.API.Infrastructure;

    using IdentityModel;

    using Nancy;

    public class UserAccessService
    {
        private readonly IUserPermissionsService userPermissionsService;

        private readonly NancyContext context;

        public UserAccessService(
            NancyContextWrapper contextWrapper,
            IUserPermissionsService userPermissionsService)
        {
            this.userPermissionsService = userPermissionsService;
            this.context = contextWrapper.Context;
        }

        public Maybe<string> GetUsername() => this.context.CurrentUser?.Identity.Name;

        public Maybe<string> GetClientId() => this.context.CurrentUser?.FindFirst(Claims.ClientId)?.Value;

        public Maybe<string> GetSubject() => this.context.CurrentUser?.FindFirst(JwtClaimTypes.Subject)?.Value;

        public Maybe<string> UserToken() =>
            this.context.Request.Headers[HttpHeaderValues.Authorization]
                .FirstMaybe()
                .Select(token => token.Substring(HttpHeaderValues.AuthorizationBearer.Length).Trim());

        public Task<bool> UserHasAccess(PermissionName permissionName) =>
            this.UserToken()
                .Select(
                    async token =>
                        {
                            var access = await this.userPermissionsService.GetPermissionsForUserViaDelegationAsync(token);

                            return !access.IsMissingRequiredPermission(permissionName);
                        })
                .Else(() => Task.FromResult(false));
    }
}