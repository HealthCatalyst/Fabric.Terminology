namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface ICodeSetCode
    {
        string Code { get; }

        string Name { get; }

        string VersionDescription { get; }

        string SourceDescription { get; }

        DateTime? RevisionDate { get; }

        ICodeSystem CodeSystem { get; }

        DateTime LastLoadDate { get; }
    }
}