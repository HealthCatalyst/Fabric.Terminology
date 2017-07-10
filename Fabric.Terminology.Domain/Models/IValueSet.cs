namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    public interface IValueSet : IValueSetMeta
    {
        string ValueSetId { get; set; }

        string Name { get; set; }

        bool IsCustom { get; }

        bool AllCodesLoaded { get; }

        int ValueSetCodesCount { get; }

        IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; }
    }
}