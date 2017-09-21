namespace Fabric.Terminology.Domain.Persistence
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Factories;

    public interface IPagingStrategy<TSrc, TResult>
        where TSrc : class, new()
    {
        int DefaultItemsPerPage { get; }

        PagedCollection<TResult> CreatePagedCollection(
            IEnumerable<TSrc> items,
            int totalCount,
            IPagerSettings pagerSettings,
            IModelFactory<TSrc, TResult> factory);

        void EnsurePagerSettings(IPagerSettings pagerSettings);
    }
}