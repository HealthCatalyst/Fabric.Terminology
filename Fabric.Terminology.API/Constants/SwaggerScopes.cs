namespace Fabric.Terminology.API.Constants
{
#pragma warning disable CA1034 // Nested types should not be visible
    internal static class SwaggerScopes
    {
        public static class Scopes
        {
            public const string ReadScope = "fabric/terminology.read";

            public const string TempScope = "HQCATALYST\\Population Builder";

            public const string WriteScope = "fabric/terminology.write";
        }

        public static class IdentityScopes
        {
            public const string ReadScope = "fabric/identity.read";

            public const string SearchUsersScope = "fabric/identity.searchusers";

            public const string AuthReadScope = "fabrice/authroziation.read";
        }
    }
#pragma warning restore CA1034 // Nested types should not be visible
}
