namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    /// <summary>
    ///     Represents a service for interacting with ValueSets.
    /// </summary>
    public interface IValueSetService
    {
        Maybe<IValueSet> GetValueSet(Guid valueSetGuid);

        Maybe<IValueSet> GetValueSet(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids);

        Task<IReadOnlyCollection<IValueSet>> GetValueSetsListAsync(IEnumerable<Guid> valueSetGuids);

        Task<IReadOnlyCollection<IValueSet>> GetValueSetsListAsync(
            IEnumerable<Guid> valueSetGuids,
            IEnumerable<Guid> codeSystemGuids);

        Task<IReadOnlyCollection<IValueSet>> GetValueSetVersionsAsync(string valueSetReferenceId);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings settings,
            IEnumerable<ValueSetStatus> statusCodes,
            bool latestVersionsOnly = true);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            IEnumerable<ValueSetStatus> statusCodes,
            bool latestVersionsOnly = true);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<ValueSetStatus> statusCodes,
            bool latestVersionsOnly = true);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            IEnumerable<ValueSetStatus> statusCodes,
            bool latestVersionsOnly = true);
    }
}