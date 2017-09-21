using System;

namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSetBackingItem : IValueSetMeta
    {
        Guid ValueSetGuid { get; }

        string ValueSetReferenceId { get; }

        string Name { get; }

        Guid OriginGuid { get; }

        string ClientCode { get; }

        bool IsCustom { get; }
    }
}
