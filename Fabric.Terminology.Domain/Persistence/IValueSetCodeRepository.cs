using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Persistence
{
    using System;
    using System.Linq;

    public interface IValueSetCodeRepository
    {
        int CountValueSetCodes(string valueSetId, params string[] codeSystemCodes);

        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(string valueSetId, params string[] codeSystemCodes);

        Task<ILookup<string, IValueSetCode>> LookupValueSetCodes(IEnumerable<string> valueSetIds, int count = 5, params string[] codeSystemCodes);
    }
}