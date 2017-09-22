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

    internal class SqlValueSetCodeRepository : IValueSetCodeRepository
    {
        private readonly SharedContext sharedContext;

        private readonly ILogger logger;

        private readonly IValueSetCachingManager<IValueSetCode> cacheManager;

        public SqlValueSetCodeRepository(
            SharedContext sharedContext,
            ILogger logger,
            IValueSetCachingManager<IValueSetCode> cacheManager)
        {
            this.sharedContext = sharedContext;
            this.logger = logger;
            this.cacheManager = cacheManager;
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid)
        {
            return this.cacheManager
                    .GetMultipleOrQuery(valueSetGuid, this.QueryValueSetCodes)
                    .OrderBy(code => code.Name)
                    .ToList();
        }

        public Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCode>>> BuildValueSetCodesDictionary(IEnumerable<Guid> valueSetGuids)
        {
            return this.cacheManager.GetCachedValueDictionary(
                valueSetGuids,
                this.QueryValueSetCodeLookup);
        }

        private IReadOnlyCollection<IValueSetCode> QueryValueSetCodes(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeFactory();

            try
            {
                return this.sharedContext.ValueSetCodes.Where(dto => dto.ValueSetGUID == valueSetGuid)
                    .Select(dto => factory.Build(dto))
                    .ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query ValueSetCodes by ValueSetGUID");
                throw;
            }
        }

        private ILookup<Guid, IValueSetCode> QueryValueSetCodeLookup(IEnumerable<Guid> valueSetGuids)
        {
            var factory = new ValueSetCodeFactory();

            try
            {
                return this.sharedContext.ValueSetCodes.Where(dto => valueSetGuids.Contains(dto.ValueSetGUID))
                    .AsNoTracking()
                    .ToLookup(vsc => vsc.ValueSetGUID, vsc => factory.Build(vsc));
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query for ValueSetCode lookup for collection of ValueSetGUIDs");
                throw;
            }
        }
    }
}