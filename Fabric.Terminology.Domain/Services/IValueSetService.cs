namespace Fabric.Terminology.Domain.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    using JetBrains.Annotations;

    /// <summary>
    /// Represents a service for interacting with ValueSets.
    /// </summary>
    public interface IValueSetService
    {
        Maybe<IValueSet> GetValueSet(string valueSetUniqueId);

        Maybe<IValueSet> GetValueSet(string valueSetUniqueId, IEnumerable<string> codeSystemCodes);

        IReadOnlyCollection<IValueSet> GetValueSets(IEnumerable<string> valueSetUniqueIds);

        IReadOnlyCollection<IValueSet> GetValueSets(
            IEnumerable<string> valueSetUniqueIds,
            IEnumerable<string> codeSystemCodes);

        IReadOnlyCollection<IValueSet> GetValueSetSummaries(IEnumerable<string> valueSetUniqueIds);

        IReadOnlyCollection<IValueSet> GetValueSetSummaries(
            IEnumerable<string> valueSetUniqueIds,
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
            IEnumerable<ICodeSetCode> valueSetCodes);

        void Save(IValueSet valueSet);

        // Can only delete custom value sets
        void Delete(IValueSet valueSet);
    }
}