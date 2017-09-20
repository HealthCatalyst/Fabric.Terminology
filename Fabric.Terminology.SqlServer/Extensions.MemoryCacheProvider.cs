namespace Fabric.Terminology.SqlServer
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Caching;

    /// <summary>
    /// Extension methods for <see cref="IMemoryCacheProvider"/>
    /// </summary>
    public static partial class Extensions
    {
        public static T GetItem<T>(this IMemoryCacheProvider cache, string key, Func<object> getter)
            where T : class
        {
            return (T)cache.GetItem(key, getter);
        }

        public static Maybe<IValueSet> GetValueSet(this IMemoryCacheProvider cache, Guid valueSetGuid)
        {
            var cacheKey = CacheKeys.ValueSetKey(valueSetGuid);
            return cache.GetItem(cacheKey).OfType<IValueSet>();
        }
    }
}
