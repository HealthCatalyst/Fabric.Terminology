namespace Fabric.Terminology.Domain.Models
{
    using System;

    internal class CodeSystem : ICodeSystem
    {
        public Guid CodeSystemGuid { get; set; }

        public string Name { get; set; }
    }
}