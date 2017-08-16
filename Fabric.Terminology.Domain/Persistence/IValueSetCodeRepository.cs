namespace Fabric.Terminology.Domain.Persistence
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetCodeRepository
    {
        int CountValueSetCodes(string valueSetUniqueId, IEnumerable<string> codeSystemCodes);

        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(
            string valueSetUniqueId,
            IEnumerable<string> codeSystemCodes);

        Task<ILookup<string, IValueSetCode>> LookupValueSetCodes(
            IEnumerable<string> valueSetUniqueIds,
            IEnumerable<string> codeSystemCodes,
            int count = 5);
    }
}