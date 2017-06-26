using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fabric.Terminology.Domain;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Models.Dto;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Fabric.Terminology.SqlServer.Persistence.Mapping;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fabric.Terminology.SqlServer.Persistence
{
    using Fabric.Terminology.Domain.Persistence.Mapping;

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

        [CanBeNull]
        public IValueSet GetValueSet(string valueSetId, params string[] codeSystemCodes)
        {
            var cached = this.Cache.GetCachedValueSetWithAllCodes(valueSetId, codeSystemCodes);
            if (cached != null)
            {
                return cached;
            }

            var dto = this.DbSet
                        .Where(vs => vs.PublicFLG == "Y" && vs.StatusCD == "Active")
                        .FirstOrDefault(q => q.ValueSetID == valueSetId);

            if (dto == null) return null;

            var mapper = new ValueSetFullCodeListMapper(this.Cache, this.valueSetCodeRepository.GetValueSetCodes, codeSystemCodes);
            
            return mapper.Map(dto);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings pagerSettings, bool includeAllValueSetCodes = false, params string[] codeSystemCodes)
        {
            return this.FindValueSetsAsync(string.Empty, pagerSettings, includeAllValueSetCodes);
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings, bool includeAllValueSetCodes = false, params string[] codeSystemCodes)
        {
            var dtos = this.DbSet.Where(dto => dto.PublicFLG == "Y" && dto.StatusCD == "Active");
            if (!nameFilterText.IsNullOrWhiteSpace())
            {
                dtos = dtos.Where(dto => dto.ValueSetNM.Contains(nameFilterText));
            }

            return this.CreatePagedCollectionAsync(dtos, pagerSettings, includeAllValueSetCodes);
        }

        private async Task<PagedCollection<IValueSet>> CreatePagedCollectionAsync(
            IQueryable<ValueSetDescriptionDto> source, 
            IPagerSettings pagerSettings, 
            bool includeAllValueSetCodes = false, 
            params string[] codeSystemCodes)
        {
            this.pagingStrategy.EnsurePagerSettings(pagerSettings);

            var count = await source.CountAsync();
            var items = await source.OrderBy(this.SortExpression).Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage).Take(pagerSettings.ItemsPerPage).ToListAsync();
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
                var cachedValueSets = this.Cache.GetItems(
                    valueSetIds
                    .Select(id => CacheKeys.ValueSetKey(id, codeSystemCodes)).ToArray())
                    .Select(obj => obj as IValueSet)
                    .Where(vs => vs != null);

                valueSetIds = valueSetIds
                                .Where(id => !cachedValueSets.Select(vs => vs.ValueSetId)
                                .Contains(id)).ToArray();

                // partition query
                var lookup = await this.valueSetCodeRepository.LookupValueSetCodes(valueSetIds);

                var cachedValueSetDictionary = cachedValueSets.ToDictionary(vs => vs.ValueSetId, vs => vs);

                mapper = new ValueSetShortCodeListMapper(
                    this.Cache,
                    lookup,
                    cachedValueSetDictionary,
                    this.valueSetCodeRepository.CountValueSetCodes,
                    codeSystemCodes);
            }

            return this.pagingStrategy.CreatePagedCollection(
                items, 
                count, 
                pagerSettings, 
                mapper);
        }
    }
}