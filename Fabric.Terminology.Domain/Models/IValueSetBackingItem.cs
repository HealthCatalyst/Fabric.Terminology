namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface IValueSetBackingItem : IValueSetMeta, IHaveValueSetGuid
    {
        string ValueSetReferenceId { get; }

        string Name { get; }

        Guid OriginGuid { get; }

        ValueSetStatus StatusCode { get; }

        bool IsCustom { get; }

        bool IsLatestVersion { get; }
    }
}
