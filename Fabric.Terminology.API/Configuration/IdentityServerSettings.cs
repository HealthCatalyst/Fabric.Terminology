namespace Fabric.Terminology.API.Configuration
{
    using System.Collections.Generic;

    using NullGuard;

    public class IdentityServerSettings
    {
        [AllowNull]
        public string ServiceNameInDiscovery { get; set; }

        [AllowNull]
        public string ClientId { get; set; }

        [AllowNull]
        public string ClientSecret { get; set; }

        [AllowNull]
        public IEnumerable<string> Scopes { get; set; }
    }
}
