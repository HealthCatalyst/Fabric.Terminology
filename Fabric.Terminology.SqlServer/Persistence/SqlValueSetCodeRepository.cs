namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlValueSetCodeRepository : IValueSetCodeRepository
    {
        private readonly SharedContext sharedContext;

        private readonly Lazy<ClientTermContext> clientTermContext;

        private readonly ILogger logger;

        private readonly IMemoryCacheProvider cache;

        public SqlValueSetCodeRepository(
            SharedContext sharedContext,
            Lazy<ClientTermContext> clientTermContext,
            IMemoryCacheProvider cache,
            ILogger logger)
        {
            this.sharedContext = sharedContext;
            this.clientTermContext = clientTermContext;
            this.logger = logger;
            this.cache = cache;
        }

        public int CountValueSetCodes(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids)
        {
            return this.GetValueSetCodes(valueSetGuid, codeSystemGuids).Count();
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid)
        {
            return this.cache.GetItem<IEnumerable<IValueSetCode>>(
                CacheKeys.ValueSetCodesKey(valueSetGuid),
                () => this.QueryValueSetCodes(valueSetGuid))
                .OrderBy(code => code.Name).ToList();
        }

        public IReadOnlyCollection<IValueSetCodeCount> GetValueSetCodeCounts(Guid valueSetGuid)
        {
            return this.cache.GetItem<IReadOnlyCollection<IValueSetCodeCount>>(
                CacheKeys.ValueSetCodeCountsKey(valueSetGuid),
                () => this.QueryValueSetCodeCounts(valueSetGuid));
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid, IEnumerable<Guid> codeSystsemGuids)
        {
            var systsemGuids = codeSystsemGuids as Guid[] ?? codeSystsemGuids.ToArray();
            return systsemGuids.Any()
                       ? this.GetValueSetCodes(valueSetGuid)
                           .Where(code => systsemGuids.Contains(code.CodeSystemGuid))
                           .ToList()
                       : this.GetValueSetCodes(valueSetGuid).ToList();
        }

        public Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCodeCount>>> BuildValueSetCountsDictionary(IEnumerable<Guid> valueSetGuids)
        {
            return this.GetCachedValueDictionary(valueSetGuids, CacheKeys.ValueSetCodeCountsKey, this.QueryValueSetCodeCountLookup);
        }

        public Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCode>>> BuildValueSetCodesDictionary(IEnumerable<Guid> valueSetGuids)
        {
            return this.GetCachedValueDictionary(valueSetGuids, CacheKeys.ValueSetCodesKey, this.QueryValueSetCodeLookup);
        }

        private Task<Dictionary<Guid, IReadOnlyCollection<TResult>>> GetCachedValueDictionary<TResult>(
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

        private IReadOnlyCollection<IValueSetCode> QueryValueSetCodes(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeFactory();

            try
            {
                return this.sharedContext.ValueSetCodes.Where(dto => dto.ValueSetGUID == valueSetGuid)
                    .Select(dto => factory.Build(dto))
                    .ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query ValueSetCodes by ValueSetGUID");
                throw;
            }
        }

        private IReadOnlyCollection<IValueSetCodeCount> QueryValueSetCodeCounts(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeCountFactory();

            try
            {
                return this.sharedContext.ValueSetCounts.Where(dto => dto.ValueSetGUID == valueSetGuid)
                    .Select(dto => factory.Build(dto))
                    .ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query ValueSetCodeCounts for ValueSetGUID");
                throw;
            }
        }

        private ILookup<Guid, IValueSetCode> QueryValueSetCodeLookup(IEnumerable<Guid> valueSetGuids)
        {
            var factory = new ValueSetCodeFactory();

            try
            {
                return this.sharedContext.ValueSetCodes.Where(dto => valueSetGuids.Contains(dto.ValueSetGUID))
                    .AsNoTracking()
                    .ToLookup(vsc => vsc.ValueSetGUID, vsc => factory.Build(vsc));
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query for ValueSetCode lookup for collection of ValueSetGUIDs");
                throw;
            }
        }

        private ILookup<Guid, IValueSetCodeCount> QueryValueSetCodeCountLookup(IEnumerable<Guid> valueSetGuids)
        {
            var factory = new ValueSetCodeCountFactory();

            try
            {
                return this.sharedContext.ValueSetCounts
                    .Where(dto => valueSetGuids.Contains(dto.ValueSetGUID))
                    .AsNoTracking()
                    .ToLookup(vscc => vscc.ValueSetGUID, vscc => factory.Build(vscc));
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query for ValueSetCodeCount lookup for collection of ValueSetGUIDs");
                throw;
            }
        }
    }
}