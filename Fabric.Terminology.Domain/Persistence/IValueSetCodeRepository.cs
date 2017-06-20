using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Persistence
{
    public interface IValueSetCodeRepository
    {
        IReadOnlyCollection<IValueSetCode> GetCodesByValueSet(string valueSetId);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string codeSystemCode, IPagerSettings settings);

        // TODO remove 
        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string codeSystemCode, int currentPage, int itemsPerPage);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string[] codeSytemCodes, IPagerSettings settings);

        // TODO remove
        // Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string[] codeSytemCodes, int currentPage, int itemsPerPage);
    }
}