using System;

namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSetCode
    {
        string Code { get; }
        string ValueSetId { get; }
        string Name { get; }
        string VersionDescription { get; }
        DateTime? RevisionDate { get; }
        IValueSetCodeSystem CodeSystem { get; }
    }
}