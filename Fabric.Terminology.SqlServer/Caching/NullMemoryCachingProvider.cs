namespace Fabric.Terminology.SqlServer.Caching
{
    using System;

    using CallMeMaybe;

    using Catalyst.Infrastructure.Caching;

    public class NullMemoryCachingProvider : IMemoryCacheProvider
    {
        public void ClearAll()
        {
        }

        public void ClearItem(string key)
        {
        }

        public Maybe<TItem> GetItem<TItem>(string key) => Maybe.Not;

        public TItem GetItem<TItem>(string key, Func<TItem> getter)
        {
            var value = getter();
            if (value == null)
            {
                throw new InvalidOperationException("Attempt to cache null value.");
            }

            return value;
        }

        public TItem GetItem<TItem>(string key, Func<TItem> getter, TimeSpan? timeout, bool isSliding = true) =>
            this.GetItem(key, getter);
    }
}