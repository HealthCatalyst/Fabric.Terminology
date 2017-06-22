using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Persistence
{
    public interface IValueSetCodeRepository
    {     
        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(string valueSetId);

        Task<PagedCollection<IValueSetCode>> GetValueSetCodes(string valueSetId, IPagerSettings settings);
    }
}