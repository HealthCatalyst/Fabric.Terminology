namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    internal class CodeSystemCodeCachingManager : ICodeSystemCodeCachingManager
    {
        private readonly IMemoryCacheProvider cache;

        public CodeSystemCodeCachingManager(IMemoryCacheProvider cache)
        {
            this.cache = cache;
        }

        public ICodeSystemCode GetOrSet(Guid codeGuid, Func<Guid, ICodeSystemCode> doQuery)
        {
            return this.cache.GetItem<ICodeSystemCode>(GetCacheKey(codeGuid), () => doQuery(codeGuid));
        }

        public IReadOnlyCollection<ICodeSystemCode> GetMultipleOrQuery(
            Func<bool, Guid[], IReadOnlyCollection<ICodeSystemCode>> doQuery,
            bool includeRetired,
            params Guid[] codeGuids)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ICodeSystemCode> GetMultiple(IEnumerable<Guid> codeGuids)
        {
            throw new NotImplementedException();
        }

        private static string GetCacheKey(Guid codeGuid)
        {
            return $"{typeof(ICodeSystemCode)}-{codeGuid}";
        }
    }
}