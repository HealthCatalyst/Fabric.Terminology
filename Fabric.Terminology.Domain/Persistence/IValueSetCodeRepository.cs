using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Persistence
{
    public interface IValueSetCodeRepository
    {
        IReadOnlyCollection<IValueSetCode> GetCodesByValueSet(string valueSetId);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string codeSystemCode, IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string searchTerm, string codeSystemCode, IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string[] codeSystemCodes, IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string searchTerm, string[] codeSystemCodes, IPagerSettings settings);

    }
}