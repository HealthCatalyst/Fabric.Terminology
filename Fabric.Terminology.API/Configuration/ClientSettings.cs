namespace Fabric.Terminology.API.Configuration
{
    using NullGuard;

    public class ClientSettings
    {
        [AllowNull]
        public string ClientId { get; set; }

        [AllowNull]
        public string ClientSecret { get; set; }

        [AllowNull]
        public string ApiSecret { get; set; }
    }
}
