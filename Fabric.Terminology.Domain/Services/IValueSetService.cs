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
        IValueSet GetValueSet(string valueSetId, params string[] codeSystemCodes);

        IEnumerable<IValueSet> GetValueSets(IReadOnlyCollection<string> valueSetIds, params string[] codeSystemCodes);

        IEnumerable<IValueSet> GetValueSetSummaries(IReadOnlyCollection<string> valueSetIds, params string[] codeSystemCodes);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings, params string[] codeSystemCodes);

        Task<PagedCollection<IValueSet>> GetValueSetSummariesAsync(IPagerSettings settings, params string[] codeSystemCodes);

        Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings, bool includeAllValueSetCodes = false, params string[] codeSystemCodes);

        bool NameIsUnique(string name);

        Attempt<IValueSet> Create(string name, IValueSetMeta meta, IEnumerable<IValueSetCodeItem> valueSetCodes);

        void Save(IValueSet valueSet);

        // Can only delete custom value sets
        void Delete(IValueSet valueSet);
    }
}