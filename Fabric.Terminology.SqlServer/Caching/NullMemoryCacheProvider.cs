using System;
using JetBrains.Annotations;

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

        [CanBeNull]
        public object GetItem(string key)
        {
            return null;
        }

        public object GetItem(string key, Func<object> getItem)
        {
            return getItem.Invoke();
        }

        public object GetItem(string cacheKey, Func<object> getItem, TimeSpan? timeout, bool isSliding = false)
        {
            return getItem.Invoke();
        }
    }
}