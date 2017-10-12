namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    public interface ICodeSystemCachingManager
    {
        ICodeSystem GetOrSet(Guid codeSystemGuid, Func<Guid, ICodeSystem> doQuery);

        IReadOnlyCollection<ICodeSystem> GetMultipleOrQuery(
            Func<bool, Guid[], IReadOnlyCollection<ICodeSystem>> doQuery,
            bool includeZeroCountCodeSystems,
            params Guid[] codeSystemGuids);

        IReadOnlyCollection<ICodeSystem> GetMultiple(IEnumerable<Guid> codeSystemGuids);
    }
}