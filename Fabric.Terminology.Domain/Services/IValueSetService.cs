﻿namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    /// <summary>
    /// Represents a service for interacting with ValueSets.
    /// </summary>
    public interface IValueSetService
    {
        Maybe<IValueSet> GetValueSet(Guid valueSetGuid);

        Maybe<IValueSet> GetValueSet(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids);

        IReadOnlyCollection<IValueSet> GetValueSets(IEnumerable<Guid> valueSetGuids);

        IReadOnlyCollection<IValueSet> GetValueSets(
            IEnumerable<Guid> valueSetGuids,
            IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings, bool latestVersionsOnly = true);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            bool latestVersionsOnly = true);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids, 
            bool latestVersionsOnly = true);

        bool NameIsUnique(string name);

        Attempt<IValueSet> Create(
            string name,
            IValueSetMeta meta,
            IReadOnlyCollection<ICodeSetCode> codeSetCodes);

        /// <summary>
        /// Saves a <see cref="IValueSet"/>
        /// </summary>
        /// <param name="valueSet">The <see cref="IValueSet"/> to be saved</param>
        /// <remarks>
        /// At this point, we can only save "new" value sets.  Updates are out of scope at the moment - To be discussed.
        /// </remarks>
        void Save(IValueSet valueSet);

        // Can only delete custom value sets
        void Delete(IValueSet valueSet);

        bool ValueSetGuidIsUnique(Guid valueSetGuid);
    }
}