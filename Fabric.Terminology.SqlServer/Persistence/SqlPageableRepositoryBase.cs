using System;
using System.Linq;
using System.Linq.Expressions;

using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Fabric.Terminology.SqlServer.Persistence.Mapping;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fabric.Terminology.SqlServer.Persistence
{
    using System.Collections.Generic;

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


        protected PagedCollection<TResult> CreatePagedCollection(IEnumerable<TDto> items, int totalCount, IPagerSettings pagerSettings, IModelMapper<TDto, TResult> mapper)
        {
            return new PagedCollection<TResult>
            {
                TotalItems = totalCount,
                PagerSettings = new PagerSettings { CurrentPage = pagerSettings.CurrentPage, ItemsPerPage = pagerSettings.ItemsPerPage },
                TotalPages = (int)Math.Ceiling((double)totalCount / pagerSettings.ItemsPerPage),
                Items = items.Select(mapper.Map).ToList().AsReadOnly()
            };
        }

        protected virtual void EnsurePagerSettings(IPagerSettings pagerSettings)
        {
            if (pagerSettings.CurrentPage <= 0) pagerSettings.CurrentPage = 1;
            if (pagerSettings.ItemsPerPage < 0) pagerSettings.ItemsPerPage = this.SharedContext.Settings.DefaultItemsPerPage;
        }
    }
}