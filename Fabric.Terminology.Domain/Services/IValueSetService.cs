namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    /// <summary>
    /// Represents a service for interacting with ValueSets.
    /// </summary>
    public interface IValueSetService
    {
        Maybe<IValueSet> GetValueSet(Guid valueSetGuid);

        Maybe<IValueSet> GetValueSet(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids);

        IReadOnlyCollection<IValueSet> GetValueSets(IEnumerable<Guid> valueSetGuids);

        IReadOnlyCollection<IValueSet> GetValueSets(
            IEnumerable<Guid> valueSetGuids,
            IEnumerable<Guid> codeSystemGuids);

        IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(IEnumerable<Guid> valueSetGuid);

        IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(
            IEnumerable<Guid> valueSetGuids,
            IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(IPagerSettings settings);

        Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSet>> FindValueSetsAsync(
            string nameFilterText,
            IPagerSettings pagerSettings);

        Task<PagedCollection<IValueSet>> FindValueSetsAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSetSummary>> FindValueSetSummariesAsync(
            string nameFilterText,
            IPagerSettings pagerSettings);

        Task<PagedCollection<IValueSetSummary>> FindValueSetSummariesAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids);

        bool NameIsUnique(string name);

        Attempt<IValueSet> Create(
            string name,
            IValueSetMeta meta,
            IEnumerable<ICodeSetCode> valueSetCodes);

        void Save(IValueSet valueSet);

        // Can only delete custom value sets
        void Delete(IValueSet valueSet);
    }
}