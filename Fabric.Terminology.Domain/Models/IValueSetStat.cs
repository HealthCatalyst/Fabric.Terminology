namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface IValueSetStat
    {
        Guid ValueSetGuid { get; }

        string CodeSystemCode { get; }

        int CodeSystemCodeCount { get; }
    }
}