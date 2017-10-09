using System;
using System.Collections.Generic;

namespace Fabric.Terminology.Domain.Services
{
    using Fabric.Terminology.Domain.Models;

    public interface IClientTermValueSetService
    {
        bool NameIsUnique(string name);

        Attempt<IValueSet> Create(string name, IValueSetMeta meta, IReadOnlyCollection<ICodeSetCode> codeSetCodes);

        /// <summary>
        ///     Saves a <see cref="IValueSet" />
        /// </summary>
        /// <param name="valueSet">The <see cref="IValueSet" /> to be saved</param>
        /// <remarks>
        ///     At this point, we can only save "new" value sets.  Updates are out of scope at the moment - To be discussed.
        /// </remarks>
        void Save(IValueSet valueSet);

        // Can only delete custom value sets
        void Delete(IValueSet valueSet);

        bool ValueSetGuidIsUnique(Guid valueSetGuid);
    }
}
