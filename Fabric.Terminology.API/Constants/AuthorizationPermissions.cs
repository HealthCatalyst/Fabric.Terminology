namespace Fabric.Terminology.API.Constants
{
    using Catalyst.DosApi.Authorization.Compliance;

    public static class AuthorizationPermissions
    {
        /// <summary>
        /// Gets the "Accessor" permission
        /// </summary>
        public static PermissionName Accessor => PermissionName.Create("dos/valuesets.accessor");

        /// <summary>
        /// Gets the "Publisher" permssion
        /// </summary>
        public static PermissionName Publisher => PermissionName.Create("dos/valuesets.publisher");
    }
}