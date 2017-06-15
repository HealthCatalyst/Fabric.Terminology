using System;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Caching;
using Microsoft.EntityFrameworkCore;

namespace Fabric.Terminology.SqlServer.Persistence
{
    internal abstract class SqlPageableRepositoryBase<TDto> : PageableRepositoryBase<TDto>
        where TDto : class
    {
        private readonly IMemoryCacheProvider _cache;

        // TODO we should inject a logger here for error catching and debuging.  Discuss with JJ
        // private readonly ILog _logger;

        protected SqlPageableRepositoryBase(SharedContext sharedContext, IMemoryCacheProvider cache)
        {
            
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.SharedContext = sharedContext ?? throw new ArgumentException(nameof(sharedContext));
        }

        protected SharedContext SharedContext { get; }

        protected abstract DbSet<TDto> DbSet { get; }
    }
}