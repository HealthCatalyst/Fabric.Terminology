namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface ICodeSystemCode : IHaveCodeGuid
    {
        string Code { get; }

        string Name { get; }

        Guid CodeSystemGuid { get; set; }

        string CodeSystemName { get; set; }
    }
}