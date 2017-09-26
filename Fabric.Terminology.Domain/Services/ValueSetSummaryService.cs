namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;

    using Serilog;

    public class ValueSetSummaryService : IValueSetSummaryService
    {
        private readonly IValueSetBackingItemRepository valueSetBackingItemRepository;

        private readonly IValueSetCodeCountRepository valueSetCodeCountRepository;

        private readonly ILogger logger;

        public ValueSetSummaryService(
            ILogger logger,
            IValueSetBackingItemRepository valueSetBackingItemRepository,
            IValueSetCodeCountRepository valueSetCodeCountRepository)
        {
            this.valueSetBackingItemRepository = valueSetBackingItemRepository;
            this.valueSetCodeCountRepository = valueSetCodeCountRepository;
            this.logger = logger;
        }

        public Maybe<IValueSetSummary> GetValueSetSummary(Guid valueSetGuid)
        {
            return this.GetValueSetSummary(valueSetGuid, new List<Guid>());
        }

        public Maybe<IValueSetSummary> GetValueSetSummary(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids)
        {
            return this.valueSetBackingItemRepository.GetValueSetBackingItem(valueSetGuid, codeSystemGuids)
                .Select(
                    backingItem =>
                        {
                            var counts = this.valueSetCodeCountRepository.GetValueSetCodeCounts(valueSetGuid);
                            return new ValueSetSummary(backingItem, counts) as IValueSetSummary;
                        });
        }

        public IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(IEnumerable<Guid> valueSetGuids)
        {
            return this.GetValueSetSummaries(valueSetGuids, new List<Guid>());
        }

        public IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(IEnumerable<Guid> valueSetGuids, IEnumerable<Guid> codeSystemGuids)
        {
            var setGuids = valueSetGuids as Guid[] ?? valueSetGuids.ToArray();
            var backingItemTask = Task.Run(
                () => this.valueSetBackingItemRepository.GetValueSetBackingItems(setGuids, codeSystemGuids));

            var countsTask = Task.Run(() => this.valueSetCodeCountRepository.BuildValueSetCountsDictionary(setGuids));

            Task.WaitAll(backingItemTask, countsTask);

            return this.BuildValueSetSummaries(backingItemTask.Result, countsTask.Result);
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(IPagerSettings settings, bool latestVersionsOnly = true)
        {
            return this.GetValueSetSummariesAsync(settings, new List<Guid>());
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true)
        {
            return this.GetValueSetSummariesAsync(string.Empty, settings, codeSystemGuids);
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            bool latestVersionsOnly = true)
        {
            return this.GetValueSetSummariesAsync(nameFilterText, pagerSettings, new List<Guid>());
        }

        public async Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true)
        {
            var backingItemPage = await this.valueSetBackingItemRepository.GetValueSetBackingItemsAsync(
                                      nameFilterText,
                                      pagerSettings,
                                      codeSystemGuids,
                                      latestVersionsOnly);

            var countsDictionary =
                await this.valueSetCodeCountRepository.BuildValueSetCountsDictionary(
                    backingItemPage.Values.Select(bi => bi.ValueSetGuid));

            return this.BuildValueSetsPage(backingItemPage, countsDictionary);
        }

        private PagedCollection<IValueSetSummary> BuildValueSetsPage(
            PagedCollection<IValueSetBackingItem> backingItemPage,
            IDictionary<Guid, IReadOnlyCollection<IValueSetCodeCount>> countsDictionary)
        {
            return new PagedCollection<IValueSetSummary>
            {
                PagerSettings = backingItemPage.PagerSettings,
                TotalItems = backingItemPage.TotalItems,
                TotalPages = backingItemPage.TotalPages,
                Values = this.BuildValueSetSummaries(backingItemPage.Values, countsDictionary)
            };
        }

        private IReadOnlyCollection<IValueSetSummary> BuildValueSetSummaries(
            IReadOnlyCollection<IValueSetBackingItem> backingItems,
            IDictionary<Guid, IReadOnlyCollection<IValueSetCodeCount>> countsDictionary)
        {
            var valueSets = backingItems.SelectMany(
                    backingItem => countsDictionary.GetMaybe(backingItem.ValueSetGuid)
                        .Select(counts => new ValueSetSummary(backingItem, counts)))
                .ToList();

            if (valueSets.Count == backingItems.Count)
            {
                return valueSets;
            }

            var vsex = new ValueSetOperationException(
                "Failed to find ValueSetCodeCounts collection for every ValueSetBackingItem (ValueSetDescription)");

            this.logger.Error(vsex, "Failed to match codes with every valueset.");
            throw vsex;
        }
    }
}