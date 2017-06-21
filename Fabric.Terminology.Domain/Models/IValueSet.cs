using System.Collections.Generic;

namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSet : IValueSetItem
    {
        string AuthoringSourceDescription { get; }
        string PurposeDescription { get; }
        string SourceDescription { get; }
        string VersionDescription { get; }

        int ValueSetCodesCount { get; }
        IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; }
    }
}