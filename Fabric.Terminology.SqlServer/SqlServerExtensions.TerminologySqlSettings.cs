namespace Fabric.Terminology.SqlServer
{
    using Catalyst.Infrastructure.Caching;

    using Fabric.Terminology.SqlServer.Configuration;

    public static partial class SqlServerExtensions
    {
        public static MemoryCacheProviderDefaultSettings AsMemoryCacheProviderSettings(
            this TerminologySqlSettings settings) =>
            new MemoryCacheProviderDefaultSettings()
            {
                DefaultCacheTimeOutMinutes = settings.MemoryCacheMinDuration,
                DefaultToSlidingExpiration = settings.MemoryCacheSliding
            };
    }
}
