namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Factories;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlValueSetCodeRepository : IValueSetCodeRepository
    {
        private readonly IValueSetCachingManager<IValueSetCode> cacheManager;

        private readonly ILogger logger;

        private readonly IPagingStrategyFactory pagingStrategyFactory;

        private readonly SharedContext sharedContext;

        public SqlValueSetCodeRepository(
            SharedContext sharedContext,
            ILogger logger,
            ICachingManagerFactory cachingManagerFactory,
            IPagingStrategyFactory pagingStrategyFactory)
        {
            this.sharedContext = sharedContext;
            this.logger = logger;
            this.pagingStrategyFactory = pagingStrategyFactory;
            this.cacheManager = cachingManagerFactory.ResolveFor<IValueSetCode>();
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid)
        {
            return this.cacheManager.GetMultipleOrQuery(valueSetGuid, this.QueryValueSetCodes)
                .OrderBy(code => code.Name)
                .ToList();
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodesByCodeGuid(Guid codeGuid)
        {
            try
            {
                var factory = new ValueSetCodeFactory();
                return this.sharedContext.ValueSetCodes.Where(dto => dto.CodeGUID == codeGuid)
                    .AsNoTracking()
                    .ToList()
                    .Select(factory.Build)
                    .ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query ValueSetCodes by CodeGuid");
                throw;
            }
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            string filterText,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids)
        {
            var dtos = this.GetValueSetCodeQueryable(filterText, settings, codeSystemGuids);

            return this.CreatePagedCollectionAsync(dtos, settings);
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            string filterText,
            Guid valueSetGuid,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids)
        {
            var dtos = this.GetValueSetCodeQueryable(filterText, settings, codeSystemGuids);

            dtos = dtos.Where(dto => dto.ValueSetGUID == valueSetGuid);

            return this.CreatePagedCollectionAsync(dtos, settings);
        }

        public Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCode>>> BuildValueSetCodesDictionary(
            IEnumerable<Guid> valueSetGuids)
        {
            return this.cacheManager.GetCachedValueDictionary(valueSetGuids, this.QueryValueSetCodeLookup);
        }

        private IQueryable<ValueSetCodeDto> GetValueSetCodeQueryable(
            string filterText,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids)
        {
            var dtos = this.sharedContext.ValueSetCodes.AsQueryable();

            if (!filterText.IsNullOrWhiteSpace())
            {
                dtos = dtos.Where(dto => dto.CodeDSC.Contains(filterText) || dto.CodeCD.StartsWith(filterText));
            }

            var systemCodes = codeSystemGuids as Guid[] ?? codeSystemGuids.ToArray();
            if (systemCodes.Any())
            {
                dtos = dtos.Where(dto => systemCodes.Contains(dto.CodeSystemGuid));
            }

            return dtos;
        }

        private IReadOnlyCollection<IValueSetCode> QueryValueSetCodes(Guid valueSetGuid)
        {
            try
            {
                var factory = new ValueSetCodeFactory();
                return this.sharedContext.ValueSetCodes.Where(dto => dto.ValueSetGUID == valueSetGuid)
                    .AsNoTracking()
                    .ToList()
                    .Select(factory.Build)
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
            try
            {
                var factory = new ValueSetCodeFactory();
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

        private async Task<PagedCollection<IValueSetCode>> CreatePagedCollectionAsync(
            IQueryable<ValueSetCodeDto> source,
            IPagerSettings pagerSettings)
        {
            var defaultItemsPerPage = this.sharedContext.Settings.DefaultItemsPerPage;
            var pagingStrategy = this.pagingStrategyFactory.GetPagingStrategy<IValueSetCode>(defaultItemsPerPage);

            pagingStrategy.EnsurePagerSettings(pagerSettings);

            var count = await source.CountAsync();
            var items = await source.OrderBy(dto => dto.CodeDSC)
                            .Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage)
                            .Take(pagerSettings.ItemsPerPage)
                            .ToListAsync();

            var factory = new ValueSetCodeFactory();

            // TODO can't cache at this point since codeGuid is null in db.  Fixme
            return pagingStrategy.CreatePagedCollection(
                items.Select(factory.Build),
                count,
                pagerSettings);

            //return pagingStrategy.CreatePagedCollection(
            //    items.Select(i => this.cacheManager.GetOrSet(i.CodeGUID.GetValueOrDefault(), () => factory.Build(i))
            //    ).Values(),
            //    count,
            //    pagerSettings);
        }
    }
}