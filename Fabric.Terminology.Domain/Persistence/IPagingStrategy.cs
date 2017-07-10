namespace Fabric.Terminology.Domain.Persistence
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Mapping;

    public interface IPagingStrategy<TSrc, TResult>
        where TSrc : class
    {
        int DefaultItemsPerPage { get; }

        PagedCollection<TResult> CreatePagedCollection(
            IEnumerable<TSrc> items,
            int totalCount,
            IPagerSettings pagerSettings,
            IModelMapper<TSrc, TResult> mapper);

        void EnsurePagerSettings(IPagerSettings pagerSettings);
    }
}