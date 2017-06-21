using System;

namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSetCode : IValueSetCodeItem
    {
        string VersionDescription { get; }
        DateTime? RevisionDate { get; }
        IValueSetCodeSystem CodeSystem { get; }
    }
}