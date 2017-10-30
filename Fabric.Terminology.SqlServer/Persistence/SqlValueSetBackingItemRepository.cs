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
        private readonly IValueSetCachingManager<IValueSetBackingItem> cacheManager;

        private readonly ILogger logger;

        private readonly IPagingStrategyFactory pagingStrategyFactory;

        private readonly SharedContext sharedContext;

        public SqlValueSetBackingItemRepository(
            SharedContext sharedContext,
            ILogger logger,
            ICachingManagerFactory cachingManagerFactory,
            IPagingStrategyFactory pagingStrategyFactory)
        {
            this.sharedContext = sharedContext;
            this.logger = logger;
            this.cacheManager = cachingManagerFactory.ResolveFor<IValueSetBackingItem>();
            this.pagingStrategyFactory = pagingStrategyFactory;
        }

        private DbSet<ValueSetDescriptionDto> DbSet => this.sharedContext.ValueSetDescriptions;

        public bool NameExists(string name)
        {
            return this.sharedContext.ValueSetDescriptions.Any(dto => dto.ValueSetNM == name);
        }

        public bool ValueSetGuidExists(Guid valueSetGuid)
        {
            return this.sharedContext.ValueSetDescriptions.Any(dto => dto.ValueSetGUID == valueSetGuid);
        }

        public Maybe<IValueSetBackingItem> GetValueSetBackingItem(Guid valueSetGuid)
        {
            return this.GetValueSetBackingItem(valueSetGuid, new List<Guid>());
        }

        public Maybe<IValueSetBackingItem> GetValueSetBackingItem(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids)
        {
            return this.GetValueSetBackingItems(new[] { valueSetGuid }, codeSystemGuids).FirstMaybe();
        }

        public IReadOnlyCollection<IValueSetBackingItem> GetValueSetBackingItemVersions(string valueSetReferenceId)
        {
            // We have to query here since we use the Guid for the cache key but the results
            // are cached for use in subesequent requests.
            return this.QueryValueSetBackingItems(valueSetReferenceId)
                .Select(bi => this.cacheManager.GetOrSet(bi.ValueSetGuid, () => bi))
                .Values()
                .ToList();
        }

        public IReadOnlyCollection<IValueSetBackingItem> GetValueSetBackingItems(IEnumerable<Guid> valueSetGuids)
        {
            return this.GetValueSetBackingItems(valueSetGuids, new List<Guid>());
        }

        public IReadOnlyCollection<IValueSetBackingItem> GetValueSetBackingItems(
            IEnumerable<Guid> valueSetGuids,
            IEnumerable<Guid> codeSystemGuids)
        {
            var setGuids = valueSetGuids as Guid[] ?? valueSetGuids.ToArray();
            var backingItems = this.cacheManager.GetMultipleExisting(setGuids).ToList();

            var remaining = setGuids.Except(backingItems.Select(bi => bi.ValueSetGuid)).ToImmutableHashSet();
            if (!remaining.Any())
            {
                return backingItems;
            }

            backingItems.AddRange(
                this.QueryValueSetBackingItems(remaining, codeSystemGuids.ToList())
                    .Select(bi => this.cacheManager.GetOrSet(bi.ValueSetGuid, () => bi))
                    .Values());

            return backingItems;
        }

        public Task<PagedCollection<IValueSetBackingItem>> GetValueSetBackingItemsAsync(
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            ValueSetStatus statusCode = ValueSetStatus.Active,
            bool latestVersionsOnly = true)
        {
            return this.GetValueSetBackingItemsAsync(
                string.Empty,
                pagerSettings,
                codeSystemGuids,
                statusCode,
                latestVersionsOnly);
        }

        public Task<PagedCollection<IValueSetBackingItem>> GetValueSetBackingItemsAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            ValueSetStatus statusCode = ValueSetStatus.Active,
            bool latestVersionsOnly = true)
        {
            var dtos = latestVersionsOnly ? this.DbSet.Where(GetBaseExpression(statusCode)) : this.DbSet.AsQueryable();

            if (!filterText.IsNullOrWhiteSpace())
            {
                dtos = dtos.Where(
                    dto => dto.ValueSetNM.Contains(filterText) || dto.ValueSetReferenceID.StartsWith(filterText));
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

        private static Expression<Func<ValueSetDescriptionDto, bool>> GetBaseExpression(ValueSetStatus statusCode)
        {
            return baseSql => baseSql.LatestVersionFLG == "Y" && baseSql.StatusCD == statusCode.ToString();
        }

        private async Task<PagedCollection<IValueSetBackingItem>> CreatePagedCollectionAsync(
            IQueryable<ValueSetDescriptionDto> source,
            IPagerSettings pagerSettings)
        {
            var defaultItemsPerPage = this.sharedContext.Settings.DefaultItemsPerPage;
            var pagingStrategy =
                this.pagingStrategyFactory.GetPagingStrategy<IValueSetBackingItem>(defaultItemsPerPage);

            pagingStrategy.EnsurePagerSettings(pagerSettings);

            var count = await source.CountAsync();
            var items = await source.OrderBy(dto => dto.ValueSetNM)
                            .Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage)
                            .Take(pagerSettings.ItemsPerPage)
                            .ToListAsync();

            var factory = new ValueSetBackingItemFactory();

            return pagingStrategy.CreatePagedCollection(
                items.Select(i => this.cacheManager.GetOrSet(i.ValueSetGUID, () => factory.Build(i))).Values(),
                count,
                pagerSettings);
        }

        private IEnumerable<IValueSetBackingItem> QueryValueSetBackingItems(
            IReadOnlyCollection<Guid> valueSetGuids,
            IReadOnlyCollection<Guid> codeSystemGuids)
        {
            var factory = new ValueSetBackingItemFactory();

            try
            {
                if (!valueSetGuids.Any())
                {
                    return new List<IValueSetBackingItem>();
                }

                var dtos = this.DbSet.Where(dto => valueSetGuids.Contains(dto.ValueSetGUID));

                if (codeSystemGuids.Any())
                {
                    var vsGuids = this.sharedContext.ValueSetCodes
                        .Where(code => codeSystemGuids.Contains(code.CodeSystemGuid))
                        .Select(code => code.ValueSetGUID)
                        .Distinct();

                    dtos = dtos.Join(vsGuids, dto => dto.ValueSetGUID, key => key, (dto, key) => dto);
                }

                return dtos.Select(dto => factory.Build(dto)).ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query ValueSetDescriptions by collection of ValueSetGUID");
                throw;
            }
        }

        private IEnumerable<IValueSetBackingItem> QueryValueSetBackingItems(string valueSetReferenceId)
        {
            var factory = new ValueSetBackingItemFactory();

            try
            {
                return this.DbSet.Where(dto => dto.ValueSetReferenceID == valueSetReferenceId)
                    .AsNoTracking()
                    .Select(dto => factory.Build(dto))
                    .ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query ValueSetDescriptions by collection of ValueSetReferenceId");
                throw;
            }
        }
    }
}