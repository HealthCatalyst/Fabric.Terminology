namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Catalyst.Infrastructure.Caching;

    using Fabric.Terminology.Domain.Models;

    internal class CodeSystemCodeCachingManager : ICodeSystemCodeCachingManager
    {
        private readonly IMemoryCacheProvider cache;

        public CodeSystemCodeCachingManager(IMemoryCacheProvider cache)
        {
            this.cache = cache;
        }

        public Maybe<ICodeSystemCode> GetOrSet(Guid codeGuid, Func<ICodeSystemCode> getter)
        {
            return Maybe.From(this.cache.GetItem<ICodeSystemCode>(GetCacheKey(codeGuid), getter));
        }

        public Maybe<ICodeSystemCode> GetOrSet(Guid codeGuid, Func<Guid, ICodeSystemCode> doQuery)
        {
            return Maybe.From(this.cache.GetItem<ICodeSystemCode>(GetCacheKey(codeGuid), () => doQuery(codeGuid)));
        }

        public IReadOnlyCollection<ICodeSystemCode> GetMultipleOrQuery(
            Func<bool, Guid[], IReadOnlyCollection<ICodeSystemCode>> doQuery,
            bool includeRetired,
            params Guid[] codeGuids)
        {
            var codes = (includeRetired ?
                this.GetMultiple(codeGuids) :
                this.GetMultiple(codeGuids).Where(csc => !csc.Retired))
                .ToList();

            if (codes.Count() == codeGuids.Length)
            {
                return codes;
            }

            // uncached items must be queried
            var remaining = codeGuids.Except(codes.Select(csc => csc.CodeGuid)).ToArray();

            // query -> cache -> add to results
            var newlyCached = doQuery(includeRetired, remaining)
                .Select(r => this.cache.GetItem<ICodeSystemCode>(GetCacheKey(r.CodeGuid), () => r));

            codes.AddRange(newlyCached);

            return codes;
        }

        public IReadOnlyCollection<ICodeSystemCode> GetMultiple(IEnumerable<Guid> codeGuids)
        {
            return codeGuids.Select(cg => this.cache.GetItem<ICodeSystemCode>(GetCacheKey(cg))).Values().ToList();
        }

        private static string GetCacheKey(Guid codeGuid)
        {
            return $"{typeof(ICodeSystemCode)}-{codeGuid}";
        }
    }
}