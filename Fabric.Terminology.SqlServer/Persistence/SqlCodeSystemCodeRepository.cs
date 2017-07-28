namespace Fabric.Terminology.SqlServer.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    internal class SqlCodeSystemCodeRepository : ICodeSetCodeRepository
    {
        public int CountCodes(IEnumerable<string> codeSystemCodes)
        {
            throw new System.NotImplementedException();
        }

        public Task<PagedCollection<ICodeSetCode>> GetCodesAsync(IPagerSettings pagerSettings)
        {
            throw new System.NotImplementedException();
        }

        public Task<PagedCollection<ICodeSetCode>> GetCodesAsync(IPagerSettings pagerSettings, IEnumerable<string> codeSystemCodes)
        {
            throw new System.NotImplementedException();
        }
    }
}