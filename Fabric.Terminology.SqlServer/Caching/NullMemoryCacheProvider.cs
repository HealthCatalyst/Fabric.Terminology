using System;
using Fabric.Terminology.SqlServer.Configuration;
using JetBrains.Annotations;

namespace Fabric.Terminology.SqlServer.Caching
{
    public class NullMemoryCacheProvider : IMemoryCacheProvider
    {
        public NullMemoryCacheProvider(IMemoryCacheSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public IMemoryCacheSettings Settings { get; }

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