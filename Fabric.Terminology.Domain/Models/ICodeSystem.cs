namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface ICodeSystem
    {
        Guid CodeSystemGuid { get; }

        string Name { get; }

        DateTime VersionDate { get; }

        string Description { get; }

        string Copyright { get; }

        string Owner { get; }
    }
}