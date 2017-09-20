namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface IValueSetCodeCount
    {
        Guid ValueSetGuid { get; }

        Guid CodeSystemGuid { get; }

        int CodeCount { get; }
    }
}