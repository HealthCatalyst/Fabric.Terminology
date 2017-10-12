namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    public interface ICodeSystemCodeCachingManager
    {
        Maybe<ICodeSystemCode> GetOrSet(Guid codeGuid, Func<ICodeSystemCode> getter);

        Maybe<ICodeSystemCode> GetOrSet(Guid codeGuid, Func<Guid, ICodeSystemCode> doQuery);

        IReadOnlyCollection<ICodeSystemCode> GetMultipleOrQuery(
            Func<bool, Guid[], IReadOnlyCollection<ICodeSystemCode>> doQuery,
            bool includeRetired,
            params Guid[] codeGuids);

        IReadOnlyCollection<ICodeSystemCode> GetMultiple(IEnumerable<Guid> codeGuids);
    }
}