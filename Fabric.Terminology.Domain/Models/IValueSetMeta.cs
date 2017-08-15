namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface IValueSetMeta
    {
        // TODO AuthorDSC
        string AuthoringSourceDescription { get; }

        // TODO REMOVE
        string PurposeDescription { get; }

        string SourceDescription { get; }

        // TODO REmove
        string VersionDescription { get; }

        // TODO ADD
        // DateTime VersionDTS { get; }
    }
}