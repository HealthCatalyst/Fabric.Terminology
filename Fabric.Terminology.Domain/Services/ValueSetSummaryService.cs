namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;

    internal class ValueSetSummaryService : IValueSetSummaryService
    {
        private readonly IValueSetBackingItemRepository valueSetBackingItemRepository;

        private readonly IValueSetCodeCountRepository valueSetCodeCountRepository;

        public ValueSetSummaryService(
            IValueSetBackingItemRepository valueSetBackingItemRepository,
            IValueSetCodeCountRepository valueSetCodeCountRepository)
        {
            this.valueSetBackingItemRepository = valueSetBackingItemRepository;
            this.valueSetCodeCountRepository = valueSetCodeCountRepository;
        }

        public IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(IEnumerable<Guid> valueSetGuids)
        {
            return this.GetValueSetSummaries(valueSetGuids, new List<Guid>());
        }

        public IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(IEnumerable<Guid> valueSetGuids, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(IPagerSettings settings)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(IPagerSettings settings, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(string nameFilterText, IPagerSettings pagerSettings)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(string nameFilterText, IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }
    }
}