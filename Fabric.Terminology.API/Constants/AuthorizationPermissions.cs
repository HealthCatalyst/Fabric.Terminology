namespace Fabric.Terminology.API.Constants
{
    using Catalyst.DosApi.Authorization.Compliance;

    public static class AuthorizationPermissions
    {
        /// <summary>
        /// Gets the "Publisher" permssion
        /// </summary>
        public static PermissionName Publisher => PermissionName.Create("app/terminology.publisher");
    }
}