namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    public interface IValueSet : IValueSetMeta
    {
        // ValueSetGUID
        string ValueSetUniqueId { get; }

        // TODO rename to ValueSetReferenceID 
        string ValueSetId { get; }

        // TODO REMOVE
        string ValueSetOId { get; }

        string Name { get;  }

        bool IsCustom { get; }

        bool AllCodesLoaded { get; }

        int ValueSetCodesCount { get; }

        IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; }
    }
}