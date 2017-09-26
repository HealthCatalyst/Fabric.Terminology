namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Factories;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlValueSetCodeCountRepository : IValueSetCodeCountRepository
    {
        private readonly SharedContext sharedContext;

        private readonly ILogger logger;

        private readonly IValueSetCachingManager<IValueSetCodeCount> cacheManager;

        public SqlValueSetCodeCountRepository(
            SharedContext sharedContext,
            ILogger logger,
            ICachingManagerFactory cachingManagerFactory)
        {
            this.sharedContext = sharedContext;
            this.logger = logger;
            this.cacheManager = cachingManagerFactory.ResolveFor<IValueSetCodeCount>();
        }

        public IReadOnlyCollection<IValueSetCodeCount> GetValueSetCodeCounts(Guid valueSetGuid)
        {
            return this.cacheManager.GetMultipleOrQuery(valueSetGuid, this.QueryValueSetCodeCounts);
        }

        public Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCodeCount>>> BuildValueSetCountsDictionary(IEnumerable<Guid> valueSetGuids)
        {
            return this.cacheManager.GetCachedValueDictionary(valueSetGuids, this.QueryValueSetCodeCountLookup);
        }

        private ILookup<Guid, IValueSetCodeCount> QueryValueSetCodeCountLookup(IEnumerable<Guid> valueSetGuids)
        {
            var factory = new ValueSetCodeCountFactory();

            try
            {
                return this.sharedContext.ValueSetCounts
                    .Where(dto => valueSetGuids.Contains(dto.ValueSetGUID))
                    .AsNoTracking()
                    .ToLookup(vscc => vscc.ValueSetGUID, vscc => factory.Build(vscc));
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query for ValueSetCodeCount lookup for collection of ValueSetGUIDs");
                throw;
            }
        }

        private IReadOnlyCollection<IValueSetCodeCount> QueryValueSetCodeCounts(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeCountFactory();

            try
            {
                return this.sharedContext.ValueSetCounts.Where(dto => dto.ValueSetGUID == valueSetGuid)
                    .Select(dto => factory.Build(dto))
                    .ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query ValueSetCodeCounts for ValueSetGUID");
                throw;
            }
        }
    }
}