namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetCodeService
    {
        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid);

        /// <summary>
        ///     Get a collection of value set codes by the codeGuid.
        /// </summary>
        /// <remarks>
        ///     Useful for determining which value sets to which a <see cref="ICodeSystemCode" /> is associated.
        /// </remarks>
        IReadOnlyCollection<IValueSetCode> GetValueSetCodesByCodeGuid(Guid codeGuid);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(IPagerSettings settings, IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(Guid valueSetGuid, IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            Guid valueSetGuid,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            string filterText,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            string filterText,
            Guid valueSetGuid,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids);
    }
}