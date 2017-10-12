namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface IHaveCodeGuid
    {
        Guid CodeGuid { get; }
    }
}