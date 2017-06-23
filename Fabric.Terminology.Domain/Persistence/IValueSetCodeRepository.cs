using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Persistence
{
    using System.Linq;

    public interface IValueSetCodeRepository
    {
        int CountValueSetCodes(string valueSetId);

        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(string valueSetId);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodes(string valueSetId, IPagerSettings settings);

        Task<ILookup<string, IValueSetCode>> LookupValueSetCodes(IEnumerable<string> valueSetIds, int count = 5);
    }
}