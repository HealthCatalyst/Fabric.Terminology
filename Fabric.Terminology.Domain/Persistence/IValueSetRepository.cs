using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Persistence
{
    public interface IValueSetRepository
    {
        IValueSet GetValueSet(string valueSetId);        
        Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings pagerSettings);
        Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings);
    }
}