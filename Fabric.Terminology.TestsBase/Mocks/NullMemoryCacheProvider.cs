using System;
using Fabric.Terminology.SqlServer.Caching;

namespace Fabric.Terminology.TestsBase.Mocks
{
    public class NullMemoryCacheProvider : IMemoryCacheProvider
    {
        public void ClearAll()
        {            
        }

        public void ClearItem(string key)
        {         
        }

        public object GetItem(string key)
        {
            return null;
        }

        public object GetItem(string key, Func<object> getItem)
        {
            return null;
        }

        public object GetItem(string cacheKey, Func<object> getCacheItem, TimeSpan? timeout, bool isSliding = false)
        {
            return null;
        }
    }
}