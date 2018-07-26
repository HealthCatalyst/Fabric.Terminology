namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface IValueSetMeta
    {
        string ClientCode { get; }

        DateTime VersionDate { get; }

        string DefinitionDescription { get; }

        string AuthorityDescription { get; }

        string SourceDescription { get; }

        DateTime LastModifiedDate { get; }
    }
}