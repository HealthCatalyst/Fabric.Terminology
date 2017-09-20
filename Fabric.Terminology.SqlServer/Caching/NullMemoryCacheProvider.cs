namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CallMeMaybe;
    using Fabric.Terminology.SqlServer.Configuration;

    using JetBrains.Annotations;

    public class NullMemoryCacheProvider : IMemoryCacheProvider
    {
        public NullMemoryCacheProvider(IMemoryCacheSettings settings)
        {
            this.Settings = settings;
        }

        public IMemoryCacheSettings Settings { get; }

        public void ClearAll()
        {
        }

        public void ClearItem(string key)
        {
        }

        public Maybe<object> GetItem(string key)
        {
            return null;
        }

        public object GetItem(string key, Func<object> getItem)
        {
            return getItem.Invoke();
        }

        public IEnumerable<object> GetItems(params string[] cacheKeys)
        {
            return Enumerable.Empty<object>();
        }

        public object GetItem(string cacheKey, Func<object> getItem, TimeSpan? timeout, bool isSliding = false)
        {
            return getItem.Invoke();
        }
    }
}