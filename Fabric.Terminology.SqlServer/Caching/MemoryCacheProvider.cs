namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.SqlServer.Configuration;

    using Microsoft.Extensions.Caching.Memory;

    internal class MemoryCacheProvider : IMemoryCacheProvider
    {
        private readonly object locker = new object();

        private IMemoryCache memCache = Create();

        public MemoryCacheProvider(IMemoryCacheSettings settings)
        {
            this.Settings = settings;
            this.InstanceKey = Guid.NewGuid();
        }

        public IMemoryCacheSettings Settings { get; }

        //// Used in tests
        internal Guid InstanceKey { get; private set; }

        public void ClearAll()
        {
            lock (this.locker)
            {
                this.memCache.Dispose();
                this.memCache = Create();
                this.InstanceKey = Guid.NewGuid();
            }
        }

        public void ClearItem(string key)
        {
            if (this.memCache.Get(key) == null)
            {
                return;
            }

            this.memCache.Remove(key);
        }

        public Maybe<object> GetItem(string key)
        {
            this.memCache.TryGetValue(key, out object result);
            return Maybe.From(result);
        }

        public Maybe<object> GetItem(string key, Func<object> getItem)
        {
            return this.GetItem(key, getItem, TimeSpan.FromMinutes(5), true);
        }

        public IEnumerable<object> GetItems(params string[] cacheKeys)
        {
            return !cacheKeys.Any() ? Enumerable.Empty<object>() : cacheKeys.Select(this.GetItem).Values();
        }

        public Maybe<object> GetItem(string key, Func<object> getItem, TimeSpan? timeout, bool isSliding = true)
        {
            if (!this.memCache.TryGetValue(key, out object value))
            {
                value = getItem.Invoke();
                if (value != null)
                {
                    var options = new MemoryCacheEntryOptions();
                    if (timeout.HasValue)
                    {
                        if (isSliding)
                        {
                            options.SetSlidingExpiration(timeout.Value);
                        }
                        else
                        {
                            options.AbsoluteExpiration = DateTime.Now + timeout.Value;
                        }
                    }

                    this.memCache.Set(key, value, options);
                }
            }

            return Maybe.From(value);
        }

        private static IMemoryCache Create()
        {
            return new MemoryCache(new MemoryCacheOptions { ExpirationScanFrequency = TimeSpan.FromMinutes(5) });
        }
    }
}