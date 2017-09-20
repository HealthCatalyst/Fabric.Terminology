namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface ICodeSystem
    {
        Guid CodeSystemGuid { get; }

        string Name { get; }
    }
}