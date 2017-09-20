namespace Fabric.Terminology.Domain.Models
{
    using System;
    using System.Collections.Generic;

    public interface IValueSetSummary : IValueSetMeta
    {
        Guid ValueSetGuid { get; }

        string ValueSetReferenceId { get; }

        string Name { get; }

        Guid OriginGuid { get; }

        string ClientCode { get; }

        bool IsCustom { get; }

        IReadOnlyCollection<IValueSetCodeCount> CodeCounts { get; }
    }
}