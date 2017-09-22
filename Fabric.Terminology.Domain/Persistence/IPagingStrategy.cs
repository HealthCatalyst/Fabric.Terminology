namespace Fabric.Terminology.Domain.Persistence
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    public interface IPagingStrategy<TResult>
    {
        int DefaultItemsPerPage { get; }

        PagedCollection<TResult> CreatePagedCollection(
            IEnumerable<TResult> items,
            int totalCount,
            IPagerSettings pagerSettings);

        void EnsurePagerSettings(IPagerSettings pagerSettings);
    }
}