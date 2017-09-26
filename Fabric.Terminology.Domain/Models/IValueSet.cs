namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    public interface IValueSet : IValueSetSummary
    {
        IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; }
    }
}