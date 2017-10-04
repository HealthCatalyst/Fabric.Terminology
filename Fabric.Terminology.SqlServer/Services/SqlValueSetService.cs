namespace Fabric.Terminology.SqlServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Persistence;

    using Serilog;

    public class SqlValueSetService : IValueSetService
    {
        private readonly IValueSetBackingItemRepository valueSetBackingItemRepository;

        private readonly IValueSetCodeRepository valueSetCodeRepository;

        private readonly IValueSetCodeCountRepository valueSetCodeCountRepository;

        private readonly ILogger logger;

        private readonly IClientTermValueSetRepository clientTermValueSetRepository;

        public SqlValueSetService(
            ILogger logger,
            IValueSetBackingItemRepository valueSetBackingItemRepository,
            IValueSetCodeRepository valueSetCodeRepository,
            IValueSetCodeCountRepository valueSetCodeCountRepository,
            IClientTermValueSetRepository clientTermValueSetRepository)
        {
            this.valueSetBackingItemRepository = valueSetBackingItemRepository;
            this.valueSetCodeRepository = valueSetCodeRepository;
            this.valueSetCodeCountRepository = valueSetCodeCountRepository;
            this.clientTermValueSetRepository = clientTermValueSetRepository;
            this.logger = logger;
        }

        public static event EventHandler<IValueSet> Created;

        public static event EventHandler<IValueSet> Saving;

        public static event EventHandler<IValueSet> Saved;

        public static event EventHandler<IValueSet> Deleting;

        public static event EventHandler<IValueSet> Deleted;

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid)
        {
            return this.GetValueSet(valueSetGuid, new List<Guid>());
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids)
        {
            return this.valueSetBackingItemRepository.GetValueSetBackingItem(valueSetGuid, codeSystemGuids)
                .Select(
                    backingItem =>
                    {
                        var codes = this.valueSetCodeRepository.GetValueSetCodes(valueSetGuid);
                        var counts = this.valueSetCodeCountRepository.GetValueSetCodeCounts(valueSetGuid);
                        return new ValueSet(backingItem, codes, counts) as IValueSet;
                    });
        }

        public Task<IReadOnlyCollection<IValueSet>> GetValueSets(IEnumerable<Guid> valueSetGuids)
        {
            return this.GetValueSets(valueSetGuids, new List<Guid>());
        }

        public async Task<IReadOnlyCollection<IValueSet>> GetValueSets(IEnumerable<Guid> valueSetGuids, IEnumerable<Guid> codeSystemGuids)
        {
            var setGuids = valueSetGuids as Guid[] ?? valueSetGuids.ToArray();
            var backingItems = this.valueSetBackingItemRepository.GetValueSetBackingItems(setGuids, codeSystemGuids);

            var codes = await this.valueSetCodeRepository.BuildValueSetCodesDictionary(setGuids);

            var counts = await this.valueSetCodeCountRepository.BuildValueSetCountsDictionary(setGuids);

            return this.BuildValueSets(backingItems, codes, counts);
        }

        public async Task<IReadOnlyCollection<IValueSet>> GetValueSetVersions(string valueSetReferenceId)
        {
            var backingItems = this.valueSetBackingItemRepository.GetValueSetBackingItemVersions(valueSetReferenceId);

            if (!backingItems.Any())
            {
                return new List<IValueSet>();
            }

            var valueSetGuids = backingItems.Select(bi => bi.ValueSetGuid).ToList();

            var codes = await this.valueSetCodeRepository.BuildValueSetCodesDictionary(valueSetGuids);

            var counts = await this.valueSetCodeCountRepository.BuildValueSetCountsDictionary(valueSetGuids);

            return this.BuildValueSets(backingItems, codes, counts);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings, bool latestVersionsOnly = true)
        {
            return this.GetValueSetsAsync(settings, new List<Guid>(), latestVersionsOnly);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings, IEnumerable<Guid> codeSystemGuids, bool latestVersionsOnly = true)
        {
            return this.GetValueSetsAsync(string.Empty, settings, codeSystemGuids, latestVersionsOnly);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings, bool latestVersionsOnly = true)
        {
            return this.GetValueSetsAsync(nameFilterText, pagerSettings, new List<Guid>(), latestVersionsOnly);
        }

        public async Task<PagedCollection<IValueSet>> GetValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids, bool latestVersionsOnly = true)
        {
            var backingItemPage = await this.valueSetBackingItemRepository.GetValueSetBackingItemsAsync(
                                      nameFilterText,
                                      pagerSettings,
                                      codeSystemGuids,
                                      latestVersionsOnly);

            var valueSetGuids = backingItemPage.Values.Select(bi => bi.ValueSetGuid).ToList();

            var codes = await this.valueSetCodeRepository.BuildValueSetCodesDictionary(valueSetGuids);

            var counts = await this.valueSetCodeCountRepository.BuildValueSetCountsDictionary(valueSetGuids);

            return this.BuildValueSetsPage(backingItemPage, codes, counts);
        }

        public Attempt<IValueSet> Create(string name, IValueSetMeta meta, IReadOnlyCollection<ICodeSetCode> codeSetCodes)
        {
            if (!this.NameIsUnique(name))
            {
                return Attempt<IValueSet>.Failed(new ArgumentException($"A value set named '{name}' already exists."));
            }

            if (!ValidateValueSetMeta(meta, out string msg))
            {
                return Attempt<IValueSet>.Failed(new ArgumentException(msg));
            }

            var setCodes = codeSetCodes as IValueSetCode[] ?? codeSetCodes.ToArray();
            if (!setCodes.Any())
            {
                return Attempt<IValueSet>.Failed(new ArgumentException("A value set must include at least one code."));
            }

            var valueSet = new ValueSet(name, meta, codeSetCodes);
            Created?.Invoke(this, valueSet);
            return Attempt<IValueSet>.Successful(valueSet);
        }

        /// <inheritdoc cref="IValueSetService.Save"/>
        public void Save(IValueSet valueSet)
        {
            Saving?.Invoke(this, valueSet);

            var attempt = this.clientTermValueSetRepository.Add(valueSet);
            if (attempt.Success && attempt.Result.HasValue)
            {
                Saved?.Invoke(this, attempt.Result.Single());
                return;
            }

            if (!attempt.Exception.HasValue)
            {
                throw new ValueSetOperationException(
                    "An exception was not returned by the attempt to save a ValueSet but the save failed.");
            }

            throw attempt.Exception.Single();
        }

        public void Delete(IValueSet valueSet)
        {
            Deleting?.Invoke(this, valueSet);

            this.clientTermValueSetRepository.Delete(valueSet);

            Deleted?.Invoke(this, valueSet);
        }

        public bool NameIsUnique(string name)
        {
            return !this.valueSetBackingItemRepository.NameExists(name);
        }

        public bool ValueSetGuidIsUnique(Guid valueSetGuid)
        {
            return !this.valueSetBackingItemRepository.ValueSetGuidExists(valueSetGuid);
        }

        private static bool ValidateValueSetMeta(IValueSetMeta meta, out string msg)
        {
            var errors = new List<string>
            {
                ValidateProperty("AuthoringSourceDescription", meta.AuthoringSourceDescription),
                ValidateProperty("DefinitionDescription", meta.DefinitionDescription),
                ValidateProperty("SourceDescription", meta.SourceDescription)
            };

            msg = string.Join(", ", errors.Where(m => !m.IsNullOrWhiteSpace()));

            return msg.IsNullOrWhiteSpace();
        }

        private static string ValidateProperty(string propName, string value)
        {
            return value.IsNullOrWhiteSpace() ? $"The {propName} property must have a value. " : string.Empty;
        }

        private PagedCollection<IValueSet> BuildValueSetsPage(
            PagedCollection<IValueSetBackingItem> backingItemPage,
            IDictionary<Guid, IReadOnlyCollection<IValueSetCode>> codesDictionary,
            IDictionary<Guid, IReadOnlyCollection<IValueSetCodeCount>> countsDictionary)
        {
            return new PagedCollection<IValueSet>
            {
                PagerSettings = backingItemPage.PagerSettings,
                TotalItems = backingItemPage.TotalItems,
                TotalPages = backingItemPage.TotalPages,
                Values = this.BuildValueSets(backingItemPage.Values, codesDictionary, countsDictionary)
            };
        }

        private IReadOnlyCollection<IValueSet> BuildValueSets(
            IReadOnlyCollection<IValueSetBackingItem> backingItems,
            IDictionary<Guid, IReadOnlyCollection<IValueSetCode>> codesDictionary,
            IDictionary<Guid, IReadOnlyCollection<IValueSetCodeCount>> countsDictionary)
        {
            // TODO Review
            var valueSets = backingItems.SelectMany(
                    item => codesDictionary.GetMaybe(item.ValueSetGuid)
                        .Select(
                            codes => countsDictionary.GetMaybe(item.ValueSetGuid)
                                .Select(counts => new ValueSet(item, codes, counts))))
                .Values().ToList();

            if (valueSets.Count == backingItems.Count)
            {
                return valueSets;
            }

            var vsex = new ValueSetOperationException(
                "Failed to find ValueSetCode collection for every ValueSetBackingItem (ValueSetDescription)");

            this.logger.Error(vsex, "Failed to match codes with every valueset.");
            throw vsex;
        }
    }
}