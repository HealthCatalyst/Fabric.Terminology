using System;

namespace Fabric.Terminology.SqlServer.Caching
{
    public interface IMemoryCacheProvider : ICacheProvider
    {
        object GetItem(string cacheKey, Func<object> getCacheItem, TimeSpan? timeout, bool isSliding = false);
    }
}