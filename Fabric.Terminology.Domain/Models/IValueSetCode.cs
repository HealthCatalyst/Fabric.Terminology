namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface IValueSetCode
    {
        string Code { get; }

        string ValueSetUniqueId { get; }

        string ValueSetId { get; }

        string Name { get; }

        string VersionDescription { get; }

        DateTime? RevisionDate { get; }

        IValueSetCodeSystem CodeSystem { get; }
    }
}