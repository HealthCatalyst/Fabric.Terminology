namespace Fabric.Terminology.API
{
    /// <summary>
    /// Represents constants used in swagger metadata modules.
    /// </summary>
#pragma warning disable CA1034 // Nested types should not be visible
    public partial class Constants
    {
        public static class Scopes
        {
            public static readonly string ReadScope = "fabric/terminology.read";

            public static readonly string TempScope = "HQCATALYST\\Population Builder";

            public static readonly string WriteScope = "fabric/terminology.write";
        }

        public static class IdentityScopes
        {
            public static readonly string ReadScope = "fabric/identity.read";
            public static readonly string SearchUsersScope = "fabric/identity.searchusers";
        }
    }
#pragma warning restore CA1034 // Nested types should not be visible
}
