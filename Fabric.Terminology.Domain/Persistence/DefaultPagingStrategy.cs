namespace Fabric.Terminology.Domain.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Mapping;

    public class DefaultPagingStrategy<TSrc, TResult> : IPagingStrategy<TSrc, TResult>
        where TSrc : class, new()
    {
        public DefaultPagingStrategy(int defaultItemsPerPage)
        {
            this.DefaultItemsPerPage = defaultItemsPerPage;
        }

        public int DefaultItemsPerPage { get; }

        public PagedCollection<TResult> CreatePagedCollection(
            IEnumerable<TSrc> items,
            int totalCount,
            IPagerSettings pagerSettings,
            IModelMapper<TSrc, TResult> mapper)
        {
            return new PagedCollection<TResult>
            {
                TotalItems = totalCount,
                PagerSettings =
                    new PagerSettings
                    {
                        CurrentPage = pagerSettings.CurrentPage,
                        ItemsPerPage = pagerSettings.ItemsPerPage
                    },
                TotalPages = (int)Math.Ceiling((double)totalCount / pagerSettings.ItemsPerPage),
                Values = items.Select(mapper.Map).ToList().AsReadOnly()
            };
        }

        public void EnsurePagerSettings(IPagerSettings pagerSettings)
        {
            if (pagerSettings.CurrentPage <= 0)
            {
                pagerSettings.CurrentPage = 1;
            }

            if (pagerSettings.ItemsPerPage < 0)
            {
                pagerSettings.ItemsPerPage = this.DefaultItemsPerPage;
            }
        }
    }
}