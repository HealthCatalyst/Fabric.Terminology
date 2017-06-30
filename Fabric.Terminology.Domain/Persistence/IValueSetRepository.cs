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
        IValueSet GetValueSet(string valueSetId, IReadOnlyCollection<string> codeSystemCodes);

        IReadOnlyCollection<IValueSet> GetValueSets(IReadOnlyCollection<string> valueSetIds, IReadOnlyCollection<string> codeSystemCodes, bool includeAllValueSetCodes = false);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings pagerSettings, IReadOnlyCollection<string> codeSystemCodes,  bool includeAllValueSetCodes = false);

        Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings, IReadOnlyCollection<string> codeSystemCodes, bool includeAllValueSetCodes = false);
    }
}