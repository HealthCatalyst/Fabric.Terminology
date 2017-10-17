namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface IValueSetCodeCount : IHaveValueSetGuid
    {
        Guid CodeSystemGuid { get; }

        int CodeCount { get; }

        string CodeSystemName { get; }
    }
}