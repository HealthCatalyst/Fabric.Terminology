namespace Fabric.Terminology.SqlServer.Persistence.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Caching;

    internal class ValueSetCachingManager : IValueSetCachingManager
    {
        private readonly IMemoryCacheProvider cache;

        public ValueSetCachingManager(IMemoryCacheProvider cache)
        {
            this.cache = cache;
        }

        public TResult GetOrQuery<TResult>(
            Guid valueSetGuid,
            Func<Guid, string> getCacheKey,
            Func<Guid, TResult> doQuery)
            where TResult : class
        {
            return this.cache.GetItem<TResult>(CacheKeys.ValueSetCodesKey(valueSetGuid), () => doQuery(valueSetGuid));
        }

        public IReadOnlyCollection<TResult> GetMultipleWithFallBack<TResult>(
            IEnumerable<Guid> valueSetGuids,
            Func<Guid, string> getCacheKey,
            Func<IEnumerable<Guid>, ILookup<Guid, TResult>> getLookup)
            where TResult : class, IHaveValueSetGuid
        {
            var setGuids = valueSetGuids as Guid[] ?? valueSetGuids.ToArray();
            var items = this.cache.GetMultipleExisting<TResult>(setGuids, getCacheKey).ToList();

            var remaining = setGuids.Except(items.Select(bi => bi.ValueSetGuid)).ToImmutableHashSet();
            if (!remaining.Any())
            {
                return items;
            }

            items.AddRange(
                    getLookup(remaining)
                    .Select(bi => this.cache.GetItem<TResult>(CacheKeys.ValueSetBackingItemKey(bi.Key), () => bi)));

            return items;
        }

        public Task<Dictionary<Guid, IReadOnlyCollection<TResult>>> GetCachedValueDictionary<TResult>(
            IEnumerable<Guid> valueSetGuids,
            Func<Guid, string> getCacheKey,
            Func<IEnumerable<Guid>, ILookup<Guid, TResult>> doQuery)
        {
            // nothing to do
            var setKeys = valueSetGuids as Guid[] ?? valueSetGuids.ToArray();
            if (!setKeys.Any())
            {
                return Task.FromResult(new Dictionary<Guid, IReadOnlyCollection<TResult>>());
            }

            // get what we can from cache
            var codes = setKeys.SelectMany(key => this.cache.GetCachedPartialValueSetAsTuple<TResult>(key, getCacheKey)).ToDictionary(t => t.Item1, t => t.Item2);
            var remaining = setKeys.Except(codes.Select(g => g.Key)).ToImmutableHashSet();
            if (!remaining.Any())
            {
                return Task.FromResult(codes);
            }

            // Query, cache return
            return Task.Run(
                () =>
                    {
                        var lookup = doQuery(remaining);

                        // Add queried values to cache
                        foreach (var key in lookup.Select(g => g.Key))
                        {
                            codes.Add(key, this.cache.GetItem<IReadOnlyCollection<TResult>>(getCacheKey(key), () => lookup[key].ToList()));
                        }

                        return codes;
                    });
        }
    }
}