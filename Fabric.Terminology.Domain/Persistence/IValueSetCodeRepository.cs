namespace Fabric.Terminology.Domain.Persistence
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetCodeRepository
    {
        int CountValueSetCodes(string valueSetId, IEnumerable<string> codeSystemCodes);

        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(
            string valueSetId,
            IEnumerable<string> codeSystemCodes);

        Task<ILookup<string, IValueSetCode>> LookupValueSetCodes(
            IEnumerable<string> valueSetIds,
            IEnumerable<string> codeSystemCodes,
            int count = 5);
    }
}