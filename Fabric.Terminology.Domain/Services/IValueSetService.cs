using System.Collections.Generic;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Services
{
    using System;
    /// <summary>
    /// Represents a service for interacting with ValueSets.
    /// </summary>
    public interface IValueSetService 
    {
        IValueSet Create(string name, IEnumerable<IValueSetCodeItem> valueSetCodes);

        IValueSet GetValueSet(string valueSetId);

        IEnumerable<IValueSet> GetValueSets(IEnumerable<string> valueSetIds);

        PagedCollection<IValueSet> GetValueSets(IPagerSettings settings);

        // Can only delete custom value sets
        void Delete(IValueSet valueSet);

        void Save(IValueSet valueSet);
    }
}