namespace Fabric.Terminology.API.Configuration
{
    public class AuthorizationServerSettings
    {
        public bool EnableCaching { get; set; }

        public int CacheDurationSeconds { get; set; } = 60;
    }
}