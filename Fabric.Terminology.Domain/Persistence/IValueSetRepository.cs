namespace Fabric.Terminology.Domain.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    using JetBrains.Annotations;

    public interface IValueSetRepository
    {
        bool NameExists(string name);

        [CanBeNull]
        IValueSet GetValueSet(string valueSetId, IEnumerable<string> codeSystemCodes);

        IReadOnlyCollection<IValueSet> GetValueSets(
            IEnumerable<string> valueSetIds,
            IEnumerable<string> codeSystemCodes,
            bool includeAllValueSetCodes = false);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings pagerSettings,
            IEnumerable<string> codeSystemCodes,
            bool includeAllValueSetCodes = false);

        Task<PagedCollection<IValueSet>> FindValueSetsAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<string> codeSystemCodes,
            bool includeAllValueSetCodes = false);
    }
}