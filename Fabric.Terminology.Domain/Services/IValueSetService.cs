using System.Collections.Generic;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Represents a service for interacting with ValueSets.
    /// </summary>
    public interface IValueSetService 
    {
        [CanBeNull]
        IValueSet GetValueSet(string valueSetId);

        [CanBeNull]
        IValueSet GetValueSet(string valueSetId, IReadOnlyCollection<string> codeSystemCodes);

        IReadOnlyCollection<IValueSet> GetValueSets(IReadOnlyCollection<string> valueSetIds);

        IReadOnlyCollection<IValueSet> GetValueSets(IReadOnlyCollection<string> valueSetIds, IReadOnlyCollection<string> codeSystemCodes);

        IReadOnlyCollection<IValueSet> GetValueSetSummaries(IReadOnlyCollection<string> valueSetIds);

        IReadOnlyCollection<IValueSet> GetValueSetSummaries(IReadOnlyCollection<string> valueSetIds, IReadOnlyCollection<string> codeSystemCodes);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings, IReadOnlyCollection<string> codeSystemCodes);

        Task<PagedCollection<IValueSet>> GetValueSetSummariesAsync(IPagerSettings settings);

        Task<PagedCollection<IValueSet>> GetValueSetSummariesAsync(IPagerSettings settings, IReadOnlyCollection<string> codeSystemCodes);

        Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings, bool includeAllValueSetCodes = false);

        Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings, IReadOnlyCollection<string> codeSystemCodes, bool includeAllValueSetCodes = false);

        bool NameIsUnique(string name);

        Attempt<IValueSet> Create(string name, IValueSetMeta meta, IReadOnlyCollection<IValueSetCodeItem> valueSetCodes);

        void Save(IValueSet valueSet);

        // Can only delete custom value sets
        void Delete(IValueSet valueSet);
    }
}