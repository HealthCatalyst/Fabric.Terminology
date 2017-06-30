using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Persistence
{
    using System;
    using System.Linq;

    public interface IValueSetCodeRepository
    {
        int CountValueSetCodes(string valueSetId, IReadOnlyCollection<string> codeSystemCodes);

        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(string valueSetId, IReadOnlyCollection<string> codeSystemCodes);

        Task<ILookup<string, IValueSetCode>> LookupValueSetCodes(IReadOnlyCollection<string> valueSetIds, IReadOnlyCollection<string> codeSystemCodes, int count = 5);
    }
}