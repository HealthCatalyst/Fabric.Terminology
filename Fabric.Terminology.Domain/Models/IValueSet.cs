namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    public interface IValueSet : IValueSetMeta
    {
        string ValueSetUniqueId { get; }

        string ValueSetId { get; }

        string ValueSetOId { get; }

        string Name { get;  }

        bool IsCustom { get; }

        bool AllCodesLoaded { get; }

        int ValueSetCodesCount { get; }

        IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; }
    }
}