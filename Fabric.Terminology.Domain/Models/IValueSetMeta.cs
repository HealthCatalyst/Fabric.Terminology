namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface IValueSetMeta
    {
        string ClientCode { get; }

        DateTime VersionDate { get; }

        string DefinitionDescription { get; }

        string AuthoringSourceDescription { get; }

        string SourceDescription { get; }
    }
}