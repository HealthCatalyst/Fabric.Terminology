namespace Fabric.Terminology.Domain.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetSummaryRepository
    {
        IReadOnlyCollection<IValueSetSummary> GetValueSetSummary(Guid valueSetGuid);

        IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(IEnumerable<Guid> valueSetGuids, IEnumerable<Guid> codeSystemGuids);

        IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(string valueSetReferenceId);

        Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(string filterText, IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids);
    }
}
