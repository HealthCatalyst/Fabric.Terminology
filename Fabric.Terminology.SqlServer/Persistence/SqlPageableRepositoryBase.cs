using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Fabric.Terminology.SqlServer.Persistence
{
    internal abstract class SqlPageableRepositoryBase<TDto, TResult> : PageableRepositoryBase<TDto>
        where TDto : class
    {
        // private readonly ILog _logger;

        protected SqlPageableRepositoryBase(SharedContext sharedContext, IMemoryCacheProvider cache)
        {
            
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.SharedContext = sharedContext ?? throw new ArgumentException(nameof(sharedContext));
        }

        protected virtual IMemoryCacheProvider Cache { get; }

        protected SharedContext SharedContext { get; }

        protected abstract DbSet<TDto> DbSet { get; }

        protected abstract TResult MapToResult(TDto dto);

        protected virtual async Task<PagedCollection<TResult>> CreatePagedCollectionAsync(IQueryable<TDto> source, IPagerSettings pagerSettings)
        {
            var count = await source.CountAsync();
            var items = await source.OrderBy(SortExpression).Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage).Take(pagerSettings.ItemsPerPage).AsNoTracking().ToListAsync();

            return new PagedCollection<TResult>
            {
                TotalItems = count,
                PagerSettings = new PagerSettings { CurrentPage = pagerSettings.CurrentPage, ItemsPerPage = pagerSettings.ItemsPerPage },
                TotalPages = (int) Math.Ceiling((double) count / pagerSettings.ItemsPerPage),
                Items = items.Select(MapToResult).ToList().AsReadOnly()
            };
        }

        protected virtual void EnsurePagerSettings(IPagerSettings pagerSettings)
        {
            if (pagerSettings.CurrentPage <= 0) pagerSettings.CurrentPage = 1;
            if (pagerSettings.ItemsPerPage < 0) pagerSettings.ItemsPerPage = SharedContext.DefaultItemsPerPage;
        }
    }
}