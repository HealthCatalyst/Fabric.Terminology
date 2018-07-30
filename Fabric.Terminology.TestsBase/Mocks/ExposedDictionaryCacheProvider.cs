namespace Fabric.Terminology.TestsBase.Mocks
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Catalyst.Infrastructure.Caching;

    using Fabric.Terminology.SqlServer.Configuration;

    public class ExposedDictionaryCacheProvider : IMemoryCacheProvider
    {
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

        public IDictionary<string, object> CachedItems { get; } = new Dictionary<string, object>();

        public void ClearAll()
        {
            this.CachedItems.Clear();
        }

        public void ClearItem(string key)
        {
            this.CachedItems.Remove(key);
        }

        public Maybe<TItem> GetItem<TItem>(string key)
        {
            return this.CachedItems.GetMaybe(key).Select(i => (TItem)i);
        }

        public TItem GetItem<TItem>(string key, Func<TItem> getter)
         => this.GetItem<TItem>(key, getter, TimeSpan.FromMinutes(5), false);

        public TItem GetItem<TItem>(string key, Func<TItem> getter, TimeSpan? timeout, bool isSliding = true)
        {
            return this.CachedItems.GetMaybe(key)
                .Select(i => (TItem)i)
                .Else(
                    () =>
                        {
                            var item = getter();
                            if (item != null)
                            {
                                this.CachedItems.Add(key, item);
                            }

                            return item;
                        });
        }
    }
}
