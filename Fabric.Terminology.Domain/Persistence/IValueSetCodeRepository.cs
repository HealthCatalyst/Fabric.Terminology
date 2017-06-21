using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Persistence
{
    public interface IValueSetCodeRepository
    {
        IValueSetCode GetCode(string code);

        IReadOnlyCollection<IValueSetCode> GetByValueSet(string valueSetId);

        Task<PagedCollection<IValueSetCode>> GetByCodeSystemAsync(string codeSystemCode, IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> GetByCodeSystemAsync(string codeNameFilterText, string codeSystemCode, IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> GetByCodeSystemAsync(string[] codeSystemCodes, IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> GetByCodeSystemAsync(string codeNameFilterText, string[] codeSystemCodes, IPagerSettings settings);
    }
}