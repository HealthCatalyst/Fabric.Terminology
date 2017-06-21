using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Persistence
{
    public interface IValueSetRepository
    {
        IValueSet GetValueSet(string valueSetId);
        Task<IReadOnlyCollection<IValueSet>> GetValueSets(params string[] ids);
        Task<PagedCollection<IValueSet>> GetValueSets(IPagerSettings pagerSettings);
        Task<PagedCollection<IValueSet>> GetValueSets(string nameFilterText, IPagerSettings pagerSettings);
    }
}