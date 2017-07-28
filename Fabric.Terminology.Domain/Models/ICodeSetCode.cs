namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface ICodeSetCode
    {
        string Code { get; }

        string Name { get; }

        string VersionDescription { get; }

        DateTime? RevisionDate { get; }

        IValueSetCodeSystem CodeSystem { get; }
    }
}