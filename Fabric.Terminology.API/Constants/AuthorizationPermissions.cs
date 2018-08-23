namespace Fabric.Terminology.API.Constants
{
    using Catalyst.DosApi.Authorization.Compliance;

    public static class AuthorizationPermissions
    {
        /// <summary>
        /// Gets the "Access" permission
        /// </summary>
        public static PermissionName Access => PermissionName.Create("dos/valuesets.access");

        /// <summary>
        /// Gets the "Publish" permssion
        /// </summary>
        public static PermissionName Publish => PermissionName.Create("dos/valuesets.publish");
    }
}