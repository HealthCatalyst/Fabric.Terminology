using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;
using JetBrains.Annotations;

namespace Fabric.Terminology.Domain.Persistence
{
    using System;
    public interface IValueSetRepository
    {
        bool NameExists(string name);
        [CanBeNull]
        IValueSet GetValueSet(string valueSetId, params string[] codeSystemCodes);

        IReadOnlyCollection<IValueSet> GetValueSets(IEnumerable<string> valueSetIds, bool includeAllValueSetCodes = false, params string[] codeSystemCodes);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings pagerSettings,  bool includeAllValueSetCodes = false, params string[] codeSystemCodes);

        Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings, bool includeAllValueSetCodes = false, params string[] codeSystemCodes);
    }
}