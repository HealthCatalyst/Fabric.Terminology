namespace Fabric.Terminology.Domain.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    using JetBrains.Annotations;

    /// <summary>
    /// Represents a service for interacting with ValueSets.
    /// </summary>
    public interface IValueSetService
    {
        [CanBeNull]
        IValueSet GetValueSet(string valueSetId);

        [CanBeNull]
        IValueSet GetValueSet(string valueSetId, IEnumerable<string> codeSystemCodes);

        IReadOnlyCollection<IValueSet> GetValueSets(IEnumerable<string> valueSetIds);

        IReadOnlyCollection<IValueSet> GetValueSets(
            IEnumerable<string> valueSetIds,
            IEnumerable<string> codeSystemCodes);

        IReadOnlyCollection<IValueSet> GetValueSetSummaries(IEnumerable<string> valueSetIds);

        IReadOnlyCollection<IValueSet> GetValueSetSummaries(
            IEnumerable<string> valueSetIds,
            IEnumerable<string> codeSystemCodes);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings settings,
            IEnumerable<string> codeSystemCodes);

        Task<PagedCollection<IValueSet>> GetValueSetSummariesAsync(IPagerSettings settings);

        Task<PagedCollection<IValueSet>> GetValueSetSummariesAsync(
            IPagerSettings settings,
            IEnumerable<string> codeSystemCodes);

        Task<PagedCollection<IValueSet>> FindValueSetsAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            bool includeAllValueSetCodes = false);

        Task<PagedCollection<IValueSet>> FindValueSetsAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<string> codeSystemCodes,
            bool includeAllValueSetCodes = false);

        bool NameIsUnique(string name);

        Attempt<IValueSet> Create(
            string name,
            IValueSetMeta meta,
            IEnumerable<IValueSetCode> valueSetCodes);

        void Save(IValueSet valueSet);

        // Can only delete custom value sets
        void Delete(IValueSet valueSet);
    }
}