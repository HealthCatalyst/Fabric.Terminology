namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    internal class CodeSystemCachingManager : ICodeSystemCachingManager
    {
        private readonly IMemoryCacheProvider cache;

        public CodeSystemCachingManager(IMemoryCacheProvider cache)
        {
            this.cache = cache;
        }

        public Maybe<ICodeSystem> GetOrSet(Guid codeSystemGuid, Func<Guid, ICodeSystem> doQuery)
        {
            return this.cache.GetItem<ICodeSystem>(GetCacheKey(codeSystemGuid), () => doQuery(codeSystemGuid));
        }

        public IReadOnlyCollection<ICodeSystem> GetMultipleOrQuery(
            Func<bool, Guid[], IReadOnlyCollection<ICodeSystem>> doQuery,
            bool includeZeroCountCodeSystems,
            params Guid[] codeSystemGuids)
        {
            var codeSystems = codeSystemGuids.Any()
                                  ? CombineCachedAndQueried(codeSystemGuids)
                                  : doQuery(includeZeroCountCodeSystems, codeSystemGuids);

            return codeSystems.Select(cs => this.cache.GetItem<ICodeSystem>(GetCacheKey(cs.CodeSystemGuid), () => cs))
                    .Values()
                    .ToList();

            IReadOnlyCollection<ICodeSystem> CombineCachedAndQueried(Guid[] keys)
            {
                var distinct = keys.Distinct().ToArray();
                var combined = this.GetMultiple(distinct).ToList();

                var remaining = distinct.Except(combined.Select(e => e.CodeSystemGuid)).ToList();
                if (remaining.Any())
                {
                    combined.AddRange(doQuery(includeZeroCountCodeSystems, remaining.ToArray()));
                }

                return combined;
            }
        }

        public IReadOnlyCollection<ICodeSystem> GetMultiple(IEnumerable<Guid> codeSystemGuids)
        {
            var systemGuids = codeSystemGuids as Guid[] ?? codeSystemGuids.ToArray();
            return systemGuids.Any()
                       ? systemGuids.Select(key => this.cache.GetItem<ICodeSystem>(GetCacheKey(key)))
                           .Values()
                           .ToList()
                       : new List<ICodeSystem>();
        }

        private static string GetCacheKey(Guid codeSystemGuid) => $"{typeof(ICodeSystem)}-{codeSystemGuid}";
    }
}