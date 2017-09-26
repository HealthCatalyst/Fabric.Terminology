namespace Fabric.Terminology.TestsBase.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Configuration;

    public class ExposedDictionaryCacheProvider : IMemoryCacheProvider
    {
        private readonly IDictionary<string, object> cache = new Dictionary<string, object>();

        public ExposedDictionaryCacheProvider()
        {
            this.Settings = new TerminologySqlSettings
            {
                MemoryCacheEnabled = false,
                MemoryCacheMinDuration = 5,
                MemoryCacheSliding = false
            };
        }

        public IMemoryCacheSettings Settings { get; }

        public IDictionary<string, object> CachedItems => this.cache;

        public void ClearAll()
        {
            this.cache.Clear();
        }

        public void ClearItem(string key)
        {
            this.cache.Remove(key);
        }

        public Maybe<object> GetItem(string key)
        {
            return this.cache.GetMaybe(key);
        }

        public Maybe<object> GetItem(string key, Func<object> getItem)
        {
            return this.GetItem(key, getItem, TimeSpan.FromMinutes(5), false);
        }

        public IEnumerable<object> GetItems(params string[] cacheKeys)
        {
            return !cacheKeys.Any() ? Enumerable.Empty<object>() : cacheKeys.Select(this.GetItem).Values();
        }

        public Maybe<object> GetItem(string cacheKey, Func<object> getItem, TimeSpan? timeout, bool isSliding = false)
        {
            return this.cache.GetMaybe(cacheKey)
                .Else(
                    () =>
                        {
                            var item = getItem();
                            if (item != null)
                            {
                                this.cache.Add(cacheKey, item);
                            }

                            return item;
                        });
        }
    }
}
