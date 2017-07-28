namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    public interface IValueSet : IValueSetMeta
    {
        string ValueSetUniqueId { get; set; }

        string ValueSetId { get; set; }

        string ValueSetOId { get; set; }

        string Name { get; set; }

        bool IsCustom { get; }

        bool AllCodesLoaded { get; }

        int ValueSetCodesCount { get; }

        IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; }
    }
}