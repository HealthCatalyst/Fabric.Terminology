namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    public interface ICodeSystemCodeService
    {
        Maybe<ICodeSystemCode> GetCodeSystemCode(Guid codeGuid);

        Task<IReadOnlyCollection<ICodeSystemCode>> GetCodeSystemCodesListAsync(IEnumerable<Guid> codeGuids);

        Task<PagedCollection<ICodeSystemCode>> GetCodeSystemCodesAsync(IPagerSettings settings, bool includeRetired = false);

        Task<PagedCollection<ICodeSystemCode>> GetCodeSystemCodesAsync(IPagerSettings settings, IEnumerable<Guid> codeSystemGuids, bool includeRetired = false);

        Task<PagedCollection<IValueSet>> GetCodeSystemCodesAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            bool includeRetired = false);
    }
}