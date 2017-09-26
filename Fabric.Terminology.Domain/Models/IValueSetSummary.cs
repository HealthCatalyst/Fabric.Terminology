namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    public interface IValueSetSummary : IValueSetBackingItem
    {
        IReadOnlyCollection<IValueSetCodeCount> CodeCounts { get; }
    }
}