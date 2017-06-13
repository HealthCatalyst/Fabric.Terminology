using System.Collections.Generic;

namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSet : IValueSetDescription
    {
        int ValueSetCodesCount { get; }
        IEnumerable<IValueSetCode> ValueSetCodes { get; }
    }
}