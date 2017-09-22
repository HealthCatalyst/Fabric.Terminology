namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Persistence.Factories;

    internal class ValueSetCachingManager<TResult> : IValueSetCachingManager<TResult>
        where TResult : class, IHaveValueSetGuid
    {
        private readonly IMemoryCacheProvider cache;

        public ValueSetCachingManager(IMemoryCacheProvider cache)
        {
            this.cache = cache;
        }

        public TResult GetOrSet(Guid valueSetGuid, TResult value)
        {
            return this.cache.GetItem<TResult>(this.GetCacheKey(valueSetGuid), () => value);
        }

        public TResult GetOrSet(Guid valueSetGuid, Func<TResult> value)
        {
            return this.cache.GetItem<TResult>(this.GetCacheKey(valueSetGuid), value);
        }

        public TResult GetOrQuery(
            Guid valueSetGuid,
            Func<Guid, TResult> doQuery)
        {
            return this.cache.GetItem<TResult>(CacheKeys.ValueSetCodesKey(valueSetGuid), () => doQuery(valueSetGuid));
        }

        public IReadOnlyCollection<TResult> GetMultipleOrQuery(Guid valueSetGuid, Func<Guid, IReadOnlyCollection<TResult>> doQuery)
        {
            return this.cache.GetItem<IReadOnlyCollection<TResult>>(this.GetCacheKey(valueSetGuid), () => doQuery(valueSetGuid));
        }

        public IReadOnlyCollection<TResult> GetMultipleWithFallBack(
            IEnumerable<Guid> valueSetGuids,
            Func<IEnumerable<Guid>, ILookup<Guid, TResult>> getLookup)
        {
            var setGuids = valueSetGuids as Guid[] ?? valueSetGuids.ToArray();
            var items = this.GetMultipleExisting(setGuids).ToList();

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

        public Task<Dictionary<Guid, IReadOnlyCollection<TResult>>> GetCachedValueDictionary(
            IEnumerable<Guid> valueSetGuids,
            Func<IEnumerable<Guid>, ILookup<Guid, TResult>> doQuery)
        {
            // nothing to do
            var setKeys = valueSetGuids as Guid[] ?? valueSetGuids.ToArray();
            if (!setKeys.Any())
            {
                return Task.FromResult(new Dictionary<Guid, IReadOnlyCollection<TResult>>());
            }

            // get what we can from cache
            var codes = setKeys.SelectMany(this.GetCachedPartialValueSetAsTuple).ToDictionary(t => t.Item1, t => t.Item2);
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
                            codes.Add(key, this.cache.GetItem<IReadOnlyCollection<TResult>>(this.GetCacheKey(key), () => lookup[key].ToList()));
                        }

                        return codes;
                    });
        }

        public IReadOnlyCollection<TResult> GetMultipleExisting(
            IEnumerable<Guid> valueSetGuids)
        {
            return valueSetGuids.Select(key => this.cache.GetItem<TResult>(this.GetCacheKey(key))).Values().ToList();
        }

        private Maybe<Tuple<Guid, IReadOnlyCollection<TResult>>> GetCachedPartialValueSetAsTuple(Guid valueSetGuid)
        {
            return this.cache.GetItem(this.GetCacheKey(valueSetGuid))
                .OfType<IReadOnlyCollection<TResult>>()
                .Select(x => new Tuple<Guid, IReadOnlyCollection<TResult>>(valueSetGuid, x));
        }

        private string GetCacheKey(Guid valueSetGuid) => $"{typeof(TResult)}-{valueSetGuid}";
    }
}