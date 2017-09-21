namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Linq.Expressions;
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

    internal class SqlValueSetBackingItemRepository : IValueSetBackingItemRepository
    {
        private readonly SharedContext sharedContext;

        private readonly ILogger logger;

        private readonly IMemoryCacheProvider cache;

        private readonly IPagingStrategyFactory pagingStrategyFactory;

        public SqlValueSetBackingItemRepository(
            SharedContext sharedContext,
            IMemoryCacheProvider cache,
            ILogger logger,
            IPagingStrategyFactory pagingStrategyFactory)
        {
            this.sharedContext = sharedContext;
            this.logger = logger;
            this.cache = cache;
            this.pagingStrategyFactory = pagingStrategyFactory;
        }

        private DbSet<ValueSetDescriptionDto> DbSet => this.sharedContext.ValueSetDescriptions;

        public Maybe<IValueSetBackingItem> GetValueSetBackingItem(Guid valueSetGuid)
        {
            return this.GetValueSetBackingItems(new[] { valueSetGuid }).FirstMaybe();
        }

        public IReadOnlyCollection<IValueSetBackingItem> GetValueSetBackingItems(IEnumerable<Guid> valueSetGuids)
        {
            var setGuids = valueSetGuids as Guid[] ?? valueSetGuids.ToArray();
            var backingItems = this.cache.GetValueSetBackingItems(setGuids).ToList();

            var remaining = setGuids.Except(backingItems.Select(bi => bi.ValueSetGuid)).ToImmutableHashSet();
            if (!remaining.Any())
            {
                return backingItems;
            }

            backingItems.AddRange(
                this.QueryValueSetBackingItems(remaining)
                .Select(bi => this.cache.GetItem<IValueSetBackingItem>(CacheKeys.ValueSetBackingItemKey(bi.ValueSetGuid), () => bi)));

            return backingItems;
        }

        public Task<PagedCollection<IValueSetBackingItem>> GetValueSetBackingItemsAsync(IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids)
        {
            return this.GetValueSetBackingItemsAsync(string.Empty, pagerSettings, codeSystemGuids);
        }

        public Task<PagedCollection<IValueSetBackingItem>> GetValueSetBackingItemsAsync(string filterText, IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids)
        {
            var dtos = this.DbSet.Where(GetBaseExpression());
            if (!filterText.IsNullOrWhiteSpace())
            {
                // TODO need to match filtering mechanic
                dtos = dtos.Where(dto => dto.ValueSetNM.Contains(filterText) || dto.ValueSetReferenceID.StartsWith(filterText));
            }

            var systemCodes = codeSystemGuids as Guid[] ?? codeSystemGuids.ToArray();
            if (systemCodes.Any())
            {
                var vsGuids = this.sharedContext.ValueSetCodes.Where(code => systemCodes.Contains(code.CodeSystemGuid))
                    .Select(code => code.ValueSetGUID)
                    .Distinct();

                dtos = dtos.Join(vsGuids, dto => dto.ValueSetGUID, key => key, (dto, key) => dto);
            }

            return this.CreatePagedCollectionAsync(dtos, pagerSettings);
        }

        private static Expression<Func<ValueSetDescriptionDto, bool>> GetBaseExpression()
        {
            return baseSql => baseSql.LatestVersionFLG == "Y" && baseSql.StatusCD == "Active";
        }

        private async Task<PagedCollection<IValueSetBackingItem>> CreatePagedCollectionAsync(
            IQueryable<ValueSetDescriptionDto> source,
            IPagerSettings pagerSettings)
        {
            var defaultItemsPerPage = this.sharedContext.Settings.DefaultItemsPerPage;
            var pagingStrategy = this.pagingStrategyFactory.GetPagingStrategy<ValueSetDescriptionDto, IValueSetBackingItem>(defaultItemsPerPage);
            pagingStrategy.EnsurePagerSettings(pagerSettings);

            var count = await source.CountAsync();
            var items = await source.OrderBy(dto => dto.ValueSetNM)
                            .Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage)
                            .Take(pagerSettings.ItemsPerPage)
                            .ToListAsync();

            return pagingStrategy.CreatePagedCollection(items, count, pagerSettings, new ValueSetBackingItemFactory());
        }

        private IReadOnlyCollection<IValueSetBackingItem> QueryValueSetBackingItems(IReadOnlyCollection<Guid> valueSetGuids)
        {
            var factory = new ValueSetBackingItemFactory();

            try
            {
                return this.DbSet.Where(GetBaseExpression()).Where(dto => valueSetGuids.Contains(dto.ValueSetGUID))
                    .AsNoTracking()
                    .Select(dto => factory.Build(dto))
                    .ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query ValueSetDescriptions by collection of ValueSetGUID");
                throw;
            }
        }
    }
}
