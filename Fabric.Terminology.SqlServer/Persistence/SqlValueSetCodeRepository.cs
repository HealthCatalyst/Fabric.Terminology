namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Querying;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Persistence.Ordering;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlValueSetCodeRepository : IValueSetCodeRepository
    {
        private readonly IValueSetCachingManager<IValueSetCode> cacheManager;

        private readonly ILogger logger;

        private readonly IPagingStrategyFactory pagingStrategyFactory;

        private readonly IOrderingStrategyFactory orderingStrategyFactory;

        private readonly SharedContext sharedContext;

        public SqlValueSetCodeRepository(
            SharedContext sharedContext,
            ILogger logger,
            ICachingManagerFactory cachingManagerFactory,
            IPagingStrategyFactory pagingStrategyFactory,
            IOrderingStrategyFactory orderingStrategyFactory)
        {
            this.sharedContext = sharedContext;
            this.logger = logger;
            this.pagingStrategyFactory = pagingStrategyFactory;
            this.orderingStrategyFactory = orderingStrategyFactory;
            this.cacheManager = cachingManagerFactory.ResolveFor<IValueSetCode>();
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid)
        {
            return this.cacheManager.GetMultipleOrQuery(valueSetGuid, this.QueryValueSetCodes)
                .OrderBy(code => code.Name)
                .ToList();
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(IEnumerable<Guid> valueSetGuids)
        {
            return this.QueryValueSetCodes(valueSetGuids);
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
            return this.GetValueSetCodesAsync(filterText, Guid.Empty, settings, codeSystemGuids);
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            string filterText,
            Guid valueSetGuid,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids)
        {
            var dtos = this.GetValueSetCodeQueryable(filterText, codeSystemGuids);

            if (valueSetGuid != Guid.Empty)
            {
                dtos = dtos.Where(dto => dto.ValueSetGUID == valueSetGuid);
            }

            return this.CreatePagedCollectionAsync(dtos, settings);
        }

        public Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCode>>> BuildValueSetCodesDictionary(
            IEnumerable<Guid> valueSetGuids)
        {
            return this.cacheManager.GetCachedValueDictionary(valueSetGuids, this.QueryValueSetCodeLookup);
        }

        private IQueryable<ValueSetCodeDto> GetValueSetCodeQueryable(
            string filterText,
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

        private IReadOnlyCollection<IValueSetCode> QueryValueSetCodes(IEnumerable<Guid> valueSetGuids)
        {
            try
            {
                var setGuids = valueSetGuids as Guid[] ?? valueSetGuids.ToArray();
                if (!setGuids.Any())
                {
                    return new List<IValueSetCode>();
                }

                var factory = new ValueSetCodeFactory();
                return this.sharedContext.ValueSetCodes.Where(dto => setGuids.Contains(dto.ValueSetGUID))
                    .AsNoTracking()
                    .ToList()
                    .Select(factory.Build)
                    .ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query ValueSetCodes given a collection ValueSetGUID");
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
            var orderingStrategy = this.orderingStrategyFactory.GetValueSetCodeStrategy();

            pagingStrategy.EnsurePagerSettings(pagerSettings);

            var count = await source.CountAsync();

            source = orderingStrategy.SetOrdering(source, pagerSettings.AsOrderingParameters());

            var items = await source.Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage)
                            .Take(pagerSettings.ItemsPerPage)
                            .ToListAsync();

            var factory = new ValueSetCodeFactory();

            // Can't cache at this point since codeGuid can be null in db.
            // If this changes in the future, caching should/could be implemented.
            return pagingStrategy.CreatePagedCollection(
                items.Select(factory.Build),
                count,
                pagerSettings);
        }
    }
}