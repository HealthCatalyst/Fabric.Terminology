namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface ICodeSetCode
    {
        Guid CodeGuid { get; }

        string Code { get; }

        string Name { get; }

        string CodeSystemCode { get; }
    }
}