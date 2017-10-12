namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    public interface ICodeSystemCachingManager
    {
        ICodeSystem GetOrSet(Guid codeSystemGuid, Func<Guid, ICodeSystem> doQuery);

        IReadOnlyCollection<ICodeSystem> GetMultipleOrQuery(
            Func<Guid[], IReadOnlyCollection<ICodeSystem>> doQuery,
            params Guid[] codeSystemGuids);

        IReadOnlyCollection<ICodeSystem> GetMultiple(IEnumerable<Guid> codeSystemGuids);
    }
}