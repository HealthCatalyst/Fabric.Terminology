using System.Collections.Generic;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Services.Persistence
{
    public interface IValueSetRepository
    {
        IValueSet GetValueSet(string valueSetId);
        IEnumerable<IValueSet> GetValueSets(params string[] ids);
    }
}