namespace Fabric.Terminology.SqlServer.Caching
{
    using System;

    using Fabric.Terminology.Domain.Models;

    public interface IClientTermCacheManager
    {
        void Clear(Guid valueSetGuid);
    }

    internal class ClientTermCacheManager : IClientTermCacheManager
    {
        private readonly IValueSetCachingManager<IValueSetCode> codeCacheManager;

        private readonly IValueSetCachingManager<IValueSetCodeCount> countCacheManager;

        private readonly IValueSetCachingManager<IValueSetBackingItem> backingItemCacheManager;

        public ClientTermCacheManager(ICachingManagerFactory cachingManagerFactory)
        {
            this.codeCacheManager = cachingManagerFactory.ResolveFor<IValueSetCode>();
            this.countCacheManager = cachingManagerFactory.ResolveFor<IValueSetCodeCount>();
            this.backingItemCacheManager = cachingManagerFactory.ResolveFor<IValueSetBackingItem>();
        }

        public void Clear(Guid valueSetGuid)
        {
            this.codeCacheManager.Clear(valueSetGuid);
            this.countCacheManager.Clear(valueSetGuid);
            this.backingItemCacheManager.Clear(valueSetGuid);
        }
    }
}
