namespace Fabric.Terminology.API.Configuration
{
    using Fabric.Terminology.SqlServer.Configuration;

    using JetBrains.Annotations;

    using NullGuard;

    public class AppConfiguration : IAppConfiguration
    {
        [AllowNull]
        public TerminologySqlSettings TerminologySqlSettings { get; set; }

        [AllowNull]
        public HostingOptions HostingOptions { get; set; }
    }
}