namespace Fabric.Terminology.Domain.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    public interface ICodeSetCodeRepository
    {
        int CountCodes(IEnumerable<string> codeSystemCodes);

        Task<PagedCollection<ICodeSystemCode>> GetCodesAsync(IPagerSettings pagerSettings);

        Task<PagedCollection<ICodeSystemCode>> GetCodesAsync(IPagerSettings pagerSettings, IEnumerable<string> codeSystemCodes);
    }
}