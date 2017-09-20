namespace Fabric.Terminology.Domain.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetCodeRepository
    {
        int CountValueSetCodes(Guid valueSetGuid, IEnumerable<string> codeSystemCodes);

        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid, IEnumerable<Guid> codeSystsemGuids);

        Task<ILookup<Guid, IValueSetCodeCount>> LookupValueSetCounts(IEnumerable<Guid> valueSetGuids);

        Task<ILookup<string, IValueSetCode>> LookupValueSetCodes(IEnumerable<Guid> valueSetGuids);
    }
}