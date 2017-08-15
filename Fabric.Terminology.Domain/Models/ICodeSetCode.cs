namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface ICodeSetCode
    {
        string Code { get; }

        string Name { get; }

        // TODO REMOVE
        string VersionDescription { get; }

        // TODO REMOVE
        string SourceDescription { get; }

        // TODO REMOVE
        DateTime? RevisionDate { get; }

        ICodeSystem CodeSystem { get; }

        // TODO REMOVE
        DateTime LastLoadDate { get; }
    }
}