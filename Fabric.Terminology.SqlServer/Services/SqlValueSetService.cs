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
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Persistence;

    using Serilog;

    public class SqlValueSetService : IValueSetService
    {
        private readonly IValueSetBackingItemRepository valueSetBackingItemRepository;

        private readonly IValueSetCodeRepository valueSetCodeRepository;

        private readonly IValueSetCodeCountRepository valueSetCodeCountRepository;

        private readonly ILogger logger;

        public SqlValueSetService(
            ILogger logger,
            IValueSetBackingItemRepository valueSetBackingItemRepository,
            IValueSetCodeRepository valueSetCodeRepository,
            IValueSetCodeCountRepository valueSetCodeCountRepository)
        {
            this.valueSetBackingItemRepository = valueSetBackingItemRepository;
            this.valueSetCodeRepository = valueSetCodeRepository;
            this.valueSetCodeCountRepository = valueSetCodeCountRepository;
            this.logger = logger;
        }

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
                        return BuildValueSet(backingItem, codes, counts);
                    });
        }

        public Task<IReadOnlyCollection<IValueSet>> GetValueSetsListAsync(IEnumerable<Guid> valueSetGuids)
        {
            return this.GetValueSetsListAsync(valueSetGuids, new List<Guid>());
        }

        public async Task<IReadOnlyCollection<IValueSet>> GetValueSetsListAsync(
            IEnumerable<Guid> valueSetGuids,
            IEnumerable<Guid> codeSystemGuids)
        {
            var setGuids = valueSetGuids as Guid[] ?? valueSetGuids.ToArray();
            var backingItems = this.valueSetBackingItemRepository.GetValueSetBackingItems(setGuids, codeSystemGuids);

            var codes = await this.valueSetCodeRepository.BuildValueSetCodesDictionary(setGuids);

            var counts = await this.valueSetCodeCountRepository.BuildValueSetCountsDictionary(setGuids);

            return this.BuildValueSets(backingItems, codes, counts);
        }

        public async Task<IReadOnlyCollection<IValueSet>> GetValueSetVersionsAsync(string valueSetReferenceId)
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
            return this.GetValueSetsAsync(
                settings,
                new List<Guid>(),
                new List<ValueSetStatus> { ValueSetStatus.Active },
                latestVersionsOnly);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings settings,
            IEnumerable<ValueSetStatus> statusCodes,
            bool latestVersionsOnly = true)
        {
            return this.GetValueSetsAsync(settings, new List<Guid>(), statusCodes, latestVersionsOnly);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            IEnumerable<ValueSetStatus> statusCodes,
            bool latestVersionsOnly = true)
        {
            return this.GetValueSetsAsync(string.Empty, settings, codeSystemGuids, statusCodes, latestVersionsOnly);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<ValueSetStatus> statusCodes,
            bool latestVersionsOnly = true)
        {
            return this.GetValueSetsAsync(filterText, pagerSettings, new List<Guid>(), statusCodes, latestVersionsOnly);
        }

        public async Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            IEnumerable<ValueSetStatus> statusCodes,
            bool latestVersionsOnly = true)
        {
            var backingItemPage = await this.valueSetBackingItemRepository.GetValueSetBackingItemsAsync(
                                      filterText,
                                      pagerSettings,
                                      codeSystemGuids,
                                      statusCodes,
                                      latestVersionsOnly);

            var valueSetGuids = backingItemPage.Values.Select(bi => bi.ValueSetGuid).ToList();

            var codes = await this.valueSetCodeRepository.BuildValueSetCodesDictionary(valueSetGuids);

            var counts = await this.valueSetCodeCountRepository.BuildValueSetCountsDictionary(valueSetGuids);

            return this.BuildValueSetsPage(backingItemPage, codes, counts);
        }

        private static IValueSet BuildValueSet(
            IValueSetBackingItem item,
            IReadOnlyCollection<IValueSetCode> codes,
            IReadOnlyCollection<IValueSetCodeCount> counts)
        {
            return new ValueSet(item, codes, counts);
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
                                .Select(counts => BuildValueSet(item, codes, counts))))
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