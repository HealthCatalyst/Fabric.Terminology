using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fabric.Terminology.Domain;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Fabric.Terminology.SqlServer.Persistence.Mapping;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fabric.Terminology.SqlServer.Persistence
{
    internal abstract class SqlPageableRepositoryBase<TDto, TResult>
        where TDto : class
    {
        protected SqlPageableRepositoryBase(SharedContext sharedContext, ILogger logger)
        {
            this.Logger = logger;            
            this.SharedContext = sharedContext;
        }

        protected ILogger Logger { get; }

        protected SharedContext SharedContext { get; }

        protected abstract DbSet<TDto> DbSet { get; }

        /// <summary>
        /// Gets the default sort expression for repository queries based off the repository DTO.
        /// </summary>
        protected abstract Expression<Func<TDto, string>> SortExpression { get; }

        /// <summary>
        /// Gets a value designating the default sort direction for repository queries.
        /// </summary>
        protected abstract SortDirection Direction { get; }
        
        protected virtual async Task<PagedCollection<TResult>> CreatePagedCollectionAsync(IQueryable<TDto> source, IPagerSettings pagerSettings, IModelMapper<TDto, TResult> mapper)
        {
            this.EnsurePagerSettings(pagerSettings);

            var count = await source.CountAsync();
            var items = await source.OrderBy(this.SortExpression).Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage).Take(pagerSettings.ItemsPerPage).AsNoTracking().ToListAsync();

            return new PagedCollection<TResult>
            {
                TotalItems = count,
                PagerSettings = new PagerSettings { CurrentPage = pagerSettings.CurrentPage, ItemsPerPage = pagerSettings.ItemsPerPage },
                TotalPages = (int) Math.Ceiling((double) count / pagerSettings.ItemsPerPage),
                Items = items.Select(mapper.Map).ToList().AsReadOnly()
            };
        }

        protected virtual void EnsurePagerSettings(IPagerSettings pagerSettings)
        {
            if (pagerSettings.CurrentPage <= 0) pagerSettings.CurrentPage = 1;
            if (pagerSettings.ItemsPerPage < 0) pagerSettings.ItemsPerPage = SharedContext.Settings.DefaultItemsPerPage;
        }
    }
}