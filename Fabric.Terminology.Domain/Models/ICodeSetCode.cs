namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface ICodeSetCode
    {
        Guid CodeGuid { get; }

        Guid CodeSystemGuid { get; set; }

        string Code { get; }

        string Name { get; }
    }
}