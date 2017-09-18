namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface IValueSetMeta
    {
        DateTime VersionDate { get; }

        string DefinitionDescription { get; }

        string AuthoringSourceDescription { get; }

        string SourceDescription { get; }
    }
}