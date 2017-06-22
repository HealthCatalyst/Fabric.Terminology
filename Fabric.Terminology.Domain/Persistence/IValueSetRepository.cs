using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;
using JetBrains.Annotations;

namespace Fabric.Terminology.Domain.Persistence
{
    public interface IValueSetRepository
    {
        [CanBeNull]
        IValueSet GetValueSet(string valueSetId);        
        Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings pagerSettings);
        Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings);
    }
}