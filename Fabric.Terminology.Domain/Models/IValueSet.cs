namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    public interface IValueSet : IValueSetBackingItem
    {
        IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; }
    }
}