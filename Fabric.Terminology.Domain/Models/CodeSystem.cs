namespace Fabric.Terminology.Domain.Models
{
    using System;

    public class CodeSystem : ICodeSystem
    {
        public Guid CodeSystemGuid { get; set; }

        public string Name { get; set; }
    }
}