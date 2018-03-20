namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetCodeRepository
    {
        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid);

        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(IEnumerable<Guid> valueSetGuids);

        IReadOnlyCollection<IValueSetCode> GetValueSetCodesByCodeGuid(Guid codeGuid);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            string filterText,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            string filterText,
            Guid valueSetGuid,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids);

        Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCode>>> BuildValueSetCodesDictionary(
            IEnumerable<Guid> valueSetGuids);
    }
}