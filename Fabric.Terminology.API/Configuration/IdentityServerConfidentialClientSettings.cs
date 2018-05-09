namespace Fabric.Terminology.API.Configuration
{
    public class IdentityServerConfidentialClientSettings
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string[] Scopes { get; set; }
    }
}
