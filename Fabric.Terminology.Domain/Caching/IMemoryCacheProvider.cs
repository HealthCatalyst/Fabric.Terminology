using System;

namespace Fabric.Terminology.Domain.Caching
{
    public interface IMemoryCacheProvider : ICacheProvider
    {
        object GetItem(string cacheKey, Func<object> getCacheItem, TimeSpan? timeout, bool isSliding = false);
    }
}