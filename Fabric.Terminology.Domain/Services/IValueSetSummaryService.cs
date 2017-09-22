namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetSummaryService
    {
        IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(IEnumerable<Guid> valueSetGuids);

        IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(
            IEnumerable<Guid> valueSetGuids,
            IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(IPagerSettings settings);

        Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(
            string nameFilterText,
            IPagerSettings pagerSettings);

        Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids);
    }
}