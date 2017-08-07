namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Persistence.Mapping;
    using Fabric.Terminology.Domain.Strategy;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Mapping;

    using JetBrains.Annotations;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlValueSetRepository : IValueSetRepository
    {
        private readonly Lazy<ClientTermContext> clientTermContext;

        private readonly IValueSetCodeRepository valueSetCodeRepository;

        private readonly IPagingStrategy<ValueSetDescriptionDto, IValueSet> pagingStrategy;

        private readonly IIsCustomValueStrategy isCustomValue;

        public SqlValueSetRepository(
            SharedContext sharedContext,
            Lazy<ClientTermContext> clientTermContext,
            IMemoryCacheProvider cache,
            ILogger logger,
            IValueSetCodeRepository valsetCodeRepository,
            IPagingStrategy<ValueSetDescriptionDto, IValueSet> pagingStrategy,
            IIsCustomValueStrategy isCustomValueStrategy)
        {
            this.clientTermContext = clientTermContext;
            this.SharedContext = sharedContext;
            this.Logger = logger;
            this.valueSetCodeRepository = valsetCodeRepository;
            this.Cache = cache;
            this.pagingStrategy = pagingStrategy;
            this.isCustomValue = isCustomValueStrategy;
        }

        protected SharedContext SharedContext { get; }

        protected ClientTermContext ClientTermContext => this.clientTermContext.Value;

        protected ILogger Logger { get; }

        protected virtual IMemoryCacheProvider Cache { get; }

        protected Expression<Func<ValueSetDescriptionDto, string>> SortExpression => sortBy => sortBy.ValueSetNM;

        protected DbSet<ValueSetDescriptionDto> DbSet => this.SharedContext.ValueSetDescriptions;

        private bool IsTest => this.SharedContext.IsInMemory || this.ClientTermContext.IsInMemory;

        public bool NameExists(string name)
        {
            return !this.IsTest
                       ? this.DbSet.Any(dto => dto.ValueSetNM == name)
                         || this.ClientTermContext.ValueSetDescriptions.Any(dto => dto.ValueSetNM == name)
                       : this.ClientTermContext.ValueSetDescriptions.Any(dto => dto.ValueSetNM == name);
        }

        public Maybe<IValueSet> GetValueSet(string valueSetUniqueId, IEnumerable<string> codeSystemCodes)
        {
            var codeSystemCDs = codeSystemCodes as string[] ?? codeSystemCodes.ToArray();
            var cached = this.Cache.GetCachedValueSetWithAllCodes(valueSetUniqueId, codeSystemCDs);
            if (cached != null)
            {
                return Maybe.From(cached);
            }

            var dto = this.DbSet.Where(GetBaseExpression()).FirstOrDefault(vs => vs.ValueSetUniqueID == valueSetUniqueId);

            if (dto == null)
            {
                return Maybe<IValueSet>.Not;
            }

            var mapper = new ValueSetFullCodeListMapper(
                this.isCustomValue,
                this.Cache,
                this.valueSetCodeRepository.GetValueSetCodes,
                codeSystemCDs);

            return Maybe.From(mapper.Map(dto));
        }

        public IReadOnlyCollection<IValueSet> GetValueSets(
            IEnumerable<string> valueSetUniqueIds,
            IEnumerable<string> codeSystemCodes,
            bool includeAllValueSetCodes = false)
        {
            var setIds = valueSetUniqueIds as string[] ?? valueSetUniqueIds.ToArray();
            var cached = setIds.Select(vsid => this.Cache.GetCachedValueSetWithAllCodes(vsid, codeSystemCodes))
                .Where(vs => vs != null)
                .ToList();

            var remaining = setIds.Except(cached.Select(s => s.ValueSetUniqueId));

            var dtos = this.DbSet.Where(GetBaseExpression()).Where(dto => remaining.Contains(dto.ValueSetUniqueID)).ToList();

            if (dtos.Any())
            {
                var mapper = new ValueSetFullCodeListMapper(
                    this.isCustomValue,
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
            IReadOnlyCollection<string> valueSetUniqueIds,
            IPagerSettings pagerSettings,
            IEnumerable<string> codeSystemCodes,
            bool includeAllValueSetCodes = false)
        {
            if (!valueSetUniqueIds.Any())
            {
                return this.FindValueSetsAsync(string.Empty, pagerSettings, codeSystemCodes, includeAllValueSetCodes);
            }

            var dtos = this.DbSet.Where(GetBaseExpression()).Where(dto => valueSetUniqueIds.Contains(dto.ValueSetUniqueID));

            return this.CreatePagedCollectionAsync(dtos, pagerSettings, codeSystemCodes, includeAllValueSetCodes);
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<string> codeSystemCodes,
            bool includeAllValueSetCodes = false)
        {
            var dtos = this.DbSet.Where(GetBaseExpression());
            if (!filterText.IsNullOrWhiteSpace())
            {
                dtos = dtos.Where(dto => dto.ValueSetNM.Contains(filterText) || dto.ValueSetOID.StartsWith(filterText));
            }

            var systemCodes = codeSystemCodes as string[] ?? codeSystemCodes.ToArray();
            if (systemCodes.Any())
            {
                var codevsid = this.SharedContext.ValueSetCodes.Where(code => systemCodes.Contains(code.CodeSystemCD))
                    .Select(code => code.ValueSetUniqueID)
                    .Distinct();

                dtos = dtos.Join(codevsid, id => id.ValueSetUniqueID, sa => sa, (id, sa) => id);
            }

            return this.CreatePagedCollectionAsync(dtos, pagerSettings, systemCodes, includeAllValueSetCodes);
        }

        public Attempt<IValueSet> Add(IValueSet valueSet)
        {
            if (!valueSet.IsCustom)
            {
                return Attempt<IValueSet>.Failed(new ValueSetOperationException("Only custom Value Sets may be created or updated."));
            }

            valueSet.SetIdsForCustomInsert();

            var valueSetDto = valueSet.AsDto();
            var codeDtos = valueSet.ValueSetCodes.Select(code => code.AsDto());

            this.ClientTermContext.ChangeTracker.AutoDetectChangesEnabled = false;
            using (var transaction = this.ClientTermContext.Database.BeginTransaction())
            {
                try
                {
                    this.ClientTermContext.ValueSetDescriptions.Add(valueSetDto);
                    this.ClientTermContext.ValueSetCodes.AddRange(codeDtos);
                    this.ClientTermContext.SaveChanges();

                    transaction.Commit();
                    this.ClientTermContext.ChangeTracker.AutoDetectChangesEnabled = true;
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Failed to save a custom ValueSet");
                    this.ClientTermContext.ChangeTracker.AutoDetectChangesEnabled = true;
                    return Attempt<IValueSet>.Failed(new ValueSetOperationException("Failed to save a custom ValueSet", ex ), valueSet);
                }
            }

            // Get the updated ValueSet
            var added = !this.IsTest ?
                this.GetValueSet(valueSetDto.ValueSetID, Enumerable.Empty<string>()) :
                this.GetCustomValueSet(valueSetDto.ValueSetUniqueID);

            return !added.HasValue ?
                Attempt<IValueSet>.Failed(new ValueSetNotFoundException("Could not retrieved newly saved ValueSet")) :
                Attempt<IValueSet>.Successful(added.Single());
        }

        public void Delete(IValueSet valueSet)
        {
            if (!valueSet.IsCustom)
            {
                return;
            }

            var valueSetDto = this.ClientTermContext.ValueSetDescriptions.Find(valueSet.ValueSetUniqueId);
            if (valueSetDto == null)
            {
                throw new ValueSetNotFoundException($"ValueSet with UniqueID {valueSet.ValueSetUniqueId} was not found.");
            }

            var codes = this.ClientTermContext.ValueSetCodes.Where(code => code.ValueSetUniqueID == valueSetDto.ValueSetUniqueID);
            using (var transaction = this.ClientTermContext.Database.BeginTransaction())
            {
                try
                {
                    this.ClientTermContext.ValueSetCodes.RemoveRange(codes);
                    this.ClientTermContext.ValueSetDescriptions.Remove(valueSetDto);
                    this.ClientTermContext.SaveChanges();

                    transaction.Commit();

                    this.Cache.ClearItem(CacheKeys.ValueSetKey(valueSet.ValueSetId));

                    // FYI - brittle point here as we may (albeit unlikely) have cached valuesets with codesystem filters applied - thus still stored in cache.
                    // Current caching mechanism does not allow for searching keys for key pattern so we have to rely on 
                    // the cache expiration (default 5 mins).
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Failed to delete custom ValueSet");
                    throw;
                }
            }
        }

        internal Maybe<IValueSet> GetCustomValueSet(string valueSetUniqueId)
        {
            var dto = this.ClientTermContext.ValueSetDescriptions.AsNoTracking().SingleOrDefault(x => x.ValueSetUniqueID == valueSetUniqueId);

            if (dto == null)
            {
                return Maybe<IValueSet>.Not;
            }

            var mapper = new ValueSetFullCodeListMapper(
                this.isCustomValue,
                this.Cache,
                ((SqlValueSetCodeRepository)this.valueSetCodeRepository).GetCustomValueSetCodes,
                Enumerable.Empty<string>());

            return Maybe.From(mapper.Map(dto));
        }

        private static Expression<Func<ValueSetDescriptionDto, bool>> GetBaseExpression()
        {
            return baseSql => baseSql.PublicFLG == "Y" && baseSql.StatusCD == "Active";
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
                    this.isCustomValue,
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
                    this.isCustomValue,
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