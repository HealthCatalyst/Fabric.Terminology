﻿namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;
    using Catalyst.Infrastructure.Caching;

    using Fabric.Terminology.Domain.Models;

    internal class ValueSetCachingManager<TResult> : IValueSetCachingManager<TResult>
        where TResult : class, IHaveValueSetGuid
    {
        private readonly IMemoryCacheProvider cache;

        public ValueSetCachingManager(IMemoryCacheProvider cache)
        {
            this.cache = cache;
        }

        public Maybe<TResult> GetOrSet(Guid valueSetGuid, Func<TResult> value)
        {
            return this.cache.GetItem<TResult>(GetCacheKey(valueSetGuid), value);
        }

        public IReadOnlyCollection<TResult> GetMultipleOrQuery(Guid valueSetGuid, Func<Guid, IReadOnlyCollection<TResult>> doQuery)
            => this.cache.GetItem<IReadOnlyCollection<TResult>>(GetCacheKey(valueSetGuid), () => doQuery(valueSetGuid));

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
                            var values = this.cache.GetItem<IReadOnlyCollection<TResult>>(
                                GetCacheKey(key),
                                () => lookup[key].ToList());

                            codes.Add(key, values);
                        }

                        return codes;
                    });
        }

        public IReadOnlyCollection<TResult> GetMultipleExisting(IEnumerable<Guid> valueSetGuids)
        {
            return valueSetGuids.Select(key => this.cache.GetItem<TResult>(GetCacheKey(key))).Values().ToList();
        }

        public void Clear(Guid valueSetGuid)
        {
            this.cache.ClearItem(GetCacheKey(valueSetGuid));
        }

        private static string GetCacheKey(Guid valueSetGuid) => $"{typeof(TResult)}-{valueSetGuid}";

        private Maybe<Tuple<Guid, IReadOnlyCollection<TResult>>> GetCachedPartialValueSetAsTuple(Guid valueSetGuid)
        {
            return this.cache.GetItem<IReadOnlyCollection<TResult>>(GetCacheKey(valueSetGuid))
                .Select(x => new Tuple<Guid, IReadOnlyCollection<TResult>>(valueSetGuid, x));
        }
    }
}