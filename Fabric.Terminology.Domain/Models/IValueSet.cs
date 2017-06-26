using System.Collections.Generic;

namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSet
    {
        string ValueSetId { get; set; }
        string Name { get; set; }
        bool IsCustom { get; }
        string AuthoringSourceDescription { get; }
        string PurposeDescription { get; }
        string SourceDescription { get; }
        string VersionDescription { get; }

        bool AllCodesLoaded { get; }

        int ValueSetCodesCount { get; }
        IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; }
    }
}