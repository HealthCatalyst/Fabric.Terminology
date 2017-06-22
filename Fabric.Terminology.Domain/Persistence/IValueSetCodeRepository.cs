using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Persistence
{
    public interface IValueSetCodeRepository
    {
        IValueSetCode GetCode(string code, string codeSystemCode, string valueSetId);

        /// <summary>
        /// Gets the <see cref="IValueSetCode"/> with CodeCD field "equal" to <param name="code">code</param>
        /// </summary>
        /// <remarks>
        /// Codes are not gaurunteed to be unique between different code systems - this method allows for querying for a specific code
        /// and ensuring that ALL codes across ALL code systems are returned.
        /// </remarks>
        IReadOnlyCollection<IValueSetCode> GetCodes(string code);        

        Task<PagedCollection<IValueSetCode>> GetCodesAsync(IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> GetCodesAsync(string codeSystemCode, IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> GetCodesAsync(string[] codeSystemCodes, IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> FindCodesAsync(string codeNameFilterText, IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> FindCodesAsync(string codeNameFilterText, string codeSystemCode, IPagerSettings settings);

        Task<PagedCollection<IValueSetCode>> FindCodesAsync(string codeNameFilterText, string[] codeSystemCodes, IPagerSettings settings);

        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(string valueSetId);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodes(string valueSetId, IPagerSettings settings);
    }
}