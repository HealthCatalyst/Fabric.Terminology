namespace Fabric.Terminology.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Caching;

    /// <summary>
    /// Extension methods for <see cref="IMemoryCacheProvider"/>
    /// </summary>
    public static partial class Extensions
    {
        public static Maybe<T> GetItem<T>(this IMemoryCacheProvider cache, string key)
            where T : class
        {
            return cache.GetItem(key).OfType<T>();
        }

        public static T GetItem<T>(this IMemoryCacheProvider cache, string key, Func<object> getter)
            where T : class
        {
            return cache.GetItem(key, getter)
                    .OfType<T>()
                    .Else(() => throw new InvalidCastException($"Failed to get an item of type {typeof(T)}"));
        }

        internal static Maybe<IValueSet> GetValueSet(this IMemoryCacheProvider cache, Guid valueSetGuid)
        {
            var cacheKey = CacheKeys.ValueSetBackingItemKey(valueSetGuid);
            return cache.GetItem(cacheKey).OfType<IValueSet>();
        }

        //internal static IReadOnlyCollection<IValueSetBackingItem> GetValueSetBackingItems(
        //    this IMemoryCacheProvider cache,
        //    IEnumerable<Guid> valueSetGuids)
        //{
        //    return valueSetGuids.Select(
        //            key => cache.GetItem<IValueSetBackingItem>(CacheKeys.ValueSetBackingItemKey(key))
        //        ).Values().ToList();
        //}

        internal static IReadOnlyCollection<TResult> GetMultipleExisting<TResult>(
            this IMemoryCacheProvider cache,
            IEnumerable<Guid> valueSetGuids,
            Func<Guid, string> getCacheKey)
            where TResult : class
        {
            return valueSetGuids.Select(key => cache.GetItem<TResult>(getCacheKey(key))).Values().ToList();
        }


        internal static Maybe<Tuple<Guid, IReadOnlyCollection<TResult>>> GetCachedPartialValueSetAsTuple<TResult>(
            this IMemoryCacheProvider cache,
            Guid valueSetGuid,
            Func<Guid, string> getCacheKey)
        {
            var cacheKey = getCacheKey(valueSetGuid);
            return cache.GetItem(cacheKey)
                .OfType<IReadOnlyCollection<TResult>>()
                .Select(x => new Tuple<Guid, IReadOnlyCollection<TResult>>(valueSetGuid, x));
        }
    }
}
