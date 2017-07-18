﻿namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Persistence.Mapping;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Mapping;

    using JetBrains.Annotations;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlValueSetRepository : IValueSetRepository
    {
        private readonly IValueSetCodeRepository valueSetCodeRepository;

        private readonly IPagingStrategy<ValueSetDescriptionDto, IValueSet> pagingStrategy;

        public SqlValueSetRepository(
            SharedContext sharedContext,
            IMemoryCacheProvider cache,
            ILogger logger,
            IValueSetCodeRepository valsetCodeRepository,
            IPagingStrategy<ValueSetDescriptionDto, IValueSet> pagingStrategy)
        {
            this.SharedContext = sharedContext;
            this.Logger = logger;
            this.valueSetCodeRepository = valsetCodeRepository;
            this.Cache = cache;
            this.pagingStrategy = pagingStrategy;
        }

        protected SharedContext SharedContext { get; }

        protected ILogger Logger { get; }

        protected virtual IMemoryCacheProvider Cache { get; }

        protected Expression<Func<ValueSetDescriptionDto, string>> SortExpression => sortBy => sortBy.ValueSetNM;

        protected DbSet<ValueSetDescriptionDto> DbSet => this.SharedContext.ValueSetDescriptions;

        public bool NameExists(string name)
        {
            return this.DbSet.Any(dto => dto.ValueSetNM == name);
        }

        [CanBeNull]
        public IValueSet GetValueSet(string valueSetId, IEnumerable<string> codeSystemCodes)
        {
            var codeSystemCDs = codeSystemCodes as string[] ?? codeSystemCodes.ToArray();
            var cached = this.Cache.GetCachedValueSetWithAllCodes(valueSetId, codeSystemCDs);
            if (cached != null)
            {
                return cached;
            }

            var dto = this.DbSet.Where(GetBaseExpression()).FirstOrDefault(vs => vs.ValueSetID == valueSetId);

            if (dto == null) return null;

            var mapper = new ValueSetFullCodeListMapper(
                this.Cache,
                this.valueSetCodeRepository.GetValueSetCodes,
                codeSystemCDs);

            return mapper.Map(dto);
        }

        public IReadOnlyCollection<IValueSet> GetValueSets(
            IEnumerable<string> valueSetIds,
            IEnumerable<string> codeSystemCodes,
            bool includeAllValueSetCodes = false)
        {
            var setIds = valueSetIds as string[] ?? valueSetIds.ToArray();
            var cached = setIds.Select(vsid => this.Cache.GetCachedValueSetWithAllCodes(vsid, codeSystemCodes))
                .Where(vs => vs != null)
                .ToList();

            var remaining = setIds.Except(cached.Select(s => s.ValueSetId));

            var dtos = this.DbSet.Where(GetBaseExpression()).Where(dto => remaining.Contains(dto.ValueSetID)).ToList();

            if (dtos.Any())
            {
                var mapper = new ValueSetFullCodeListMapper(
                    this.Cache,
                    this.valueSetCodeRepository.GetValueSetCodes,
                    codeSystemCodes);
                cached.AddRange(dtos.Select(mapper.Map).Where(mapped => mapped != null));
            }

            return cached.OrderBy(vs => vs.Name).ToList().AsReadOnly();
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings pagerSettings,
            IEnumerable<string> codeSystemCodes,
            bool includeAllValueSetCodes = false)
        {
            return this.FindValueSetsAsync(string.Empty, pagerSettings, codeSystemCodes, includeAllValueSetCodes);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IReadOnlyCollection<string> valueSetIds,
            IPagerSettings pagerSettings,
            IEnumerable<string> codeSystemCodes,
            bool includeAllValueSetCodes = false)
        {
            if (!valueSetIds.Any())
            {
                return this.FindValueSetsAsync(string.Empty, pagerSettings, codeSystemCodes, includeAllValueSetCodes);
            }

            var dtos = this.DbSet.Where(GetBaseExpression()).Where(dto => valueSetIds.Contains(dto.ValueSetID));

            return this.CreatePagedCollectionAsync(dtos, pagerSettings, codeSystemCodes, includeAllValueSetCodes);
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<string> codeSystemCodes,
            bool includeAllValueSetCodes = false)
        {
            var dtos = this.DbSet.Where(GetBaseExpression());
            if (!nameFilterText.IsNullOrWhiteSpace())
            {
                dtos = dtos.Where(dto => dto.ValueSetNM.Contains(nameFilterText));
            }

            var systemCodes = codeSystemCodes as string[] ?? codeSystemCodes.ToArray();
            if (systemCodes.Any())
            {
                var codevsid = this.SharedContext.ValueSetCodes
                    .Where(code => systemCodes.Contains(code.CodeSystemCD))
                    .Select(code => code.ValueSetID)
                    .Distinct();

                dtos = dtos.Join(codevsid, id => id.ValueSetID, sa => sa, (id, sa) => id);
            }

            return this.CreatePagedCollectionAsync(dtos, pagerSettings, systemCodes, includeAllValueSetCodes);
        }

        private static Expression<Func<ValueSetDescriptionDto, bool>> GetBaseExpression()
        {
            return baseSql => baseSql.PublicFLG == "Y" && baseSql.StatusCD == "Active" && baseSql.LatestVersionFLG == "Y";
        }

        private async Task<PagedCollection<IValueSet>> CreatePagedCollectionAsync(
            IQueryable<ValueSetDescriptionDto> source,
            IPagerSettings pagerSettings,
            IEnumerable<string> codeSystemCodes,
            bool includeAllValueSetCodes = false)
        {
            this.pagingStrategy.EnsurePagerSettings(pagerSettings);

            var count = await source.CountAsync();
            var items = await source.OrderBy(this.SortExpression)
                            .Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage)
                            .Take(pagerSettings.ItemsPerPage)
                            .ToListAsync();
            var valueSetIds = items.Select(item => item.ValueSetID).ToArray();

            IModelMapper<ValueSetDescriptionDto, IValueSet> mapper;
            if (includeAllValueSetCodes)
            {
                mapper = new ValueSetFullCodeListMapper(
                    this.Cache,
                    this.valueSetCodeRepository.GetValueSetCodes,
                    codeSystemCodes);
            }
            else
            {
                // remove any valueSetIds for valuesets already cached from partition query
                var cachedValueSets = this.Cache
                    .GetItems(valueSetIds.Select(id => CacheKeys.ValueSetKey(id, codeSystemCodes)).ToArray())
                    .Select(obj => obj as IValueSet)
                    .Where(vs => vs != null);

                valueSetIds = valueSetIds.Where(id => !cachedValueSets.Select(vs => vs.ValueSetId).Contains(id))
                    .ToArray();

                // partition query
                var systemCodes = codeSystemCodes as string[] ?? codeSystemCodes.ToArray();
                var lookup = await this.valueSetCodeRepository.LookupValueSetCodes(valueSetIds, systemCodes);

                var cachedValueSetDictionary = cachedValueSets.ToDictionary(vs => vs.ValueSetId, vs => vs);

                mapper = new ValueSetShortCodeListMapper(
                    this.Cache,
                    lookup,
                    cachedValueSetDictionary,
                    this.valueSetCodeRepository.CountValueSetCodes,
                    systemCodes);
            }

            return this.pagingStrategy.CreatePagedCollection(items, count, pagerSettings, mapper);
        }
    }
}