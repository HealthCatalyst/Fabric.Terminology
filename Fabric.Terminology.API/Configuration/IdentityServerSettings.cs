namespace Fabric.Terminology.API.Configuration
{
    using System.Collections.Generic;

    public class IdentityServerSettings
    {
        public string ServiceNameInDiscovery { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public IEnumerable<string> Scopes { get; set; }
    }
}
