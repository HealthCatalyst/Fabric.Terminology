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
        IValueSet GetValueSet(string valueSetId, params string[] codeSystemCodes);

        IEnumerable<IValueSet> GetValueSets(IEnumerable<string> valueSetIds, params string[] codeSystemCodes);

        PagedCollection<IValueSet> GetValueSets(IPagerSettings settings, params string[] codeSystemCodes);

        bool NameIsUnique(string name);

        Attempt<IValueSet> Create(string name, IValueSetMeta meta, IEnumerable<IValueSetCodeItem> valueSetCodes);

        void Save(IValueSet valueSet);

        // Can only delete custom value sets
        void Delete(IValueSet valueSet);
    }
}