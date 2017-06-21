using System;

namespace Fabric.Terminology.SqlServer.Caching
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

        public object GetItem(string cacheKey, Func<object> getItem, TimeSpan? timeout, bool isSliding = false)
        {
            return getItem.Invoke();
        }
    }
}