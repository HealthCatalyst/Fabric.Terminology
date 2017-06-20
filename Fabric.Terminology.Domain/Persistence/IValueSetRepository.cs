using System.Collections.Generic;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Persistence
{
    public interface IValueSetRepository
    {
        IValueSet GetValueSet(string valueSetId);
        IReadOnlyCollection<IValueSet> GetValueSets(params string[] ids);
    }
}