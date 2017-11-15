namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    public interface IClientTermValueSetService
    {
        bool NameIsUnique(string name);

        Attempt<IValueSet> Create(string name, IValueSetMeta meta, IReadOnlyCollection<ICodeSystemCode> codeSetCodes);

        /// <summary>
        ///     Saves a <see cref="IValueSet" /> as a new value set
        /// </summary>
        /// <param name="valueSet">The <see cref="IValueSet" /> to be saved</param>
        /// <remarks>
        ///     At this point, we can only save "new" value sets.  Updates are out of scope at the moment - To be discussed.
        /// </remarks>
        void SaveAsNew(IValueSet valueSet);

        Attempt<IValueSet> Copy(IValueSet originalValueSet, string newName, IValueSetMeta meta);

        Attempt<IValueSet> AddRemoveCodes(
            Guid valueSetGuid,
            IEnumerable<ICodeSystemCode> codesToAdd,
            IEnumerable<ICodeSystemCode> codesToRemove);

        // Can only delete custom value sets
        void Delete(IValueSet valueSet);

        bool ValueSetGuidIsUnique(Guid valueSetGuid);
    }
}