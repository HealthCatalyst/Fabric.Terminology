namespace Fabric.Terminology.Domain.Models
{
    using System;

    internal class CodeSystem : ICodeSystem
    {
        public Guid CodeSystemGuid { get; internal set; }

        public string Name { get; internal set; }

        public DateTime VersionDate { get; internal set; }

        public string Description { get; internal set; }

        public string Copyright { get; internal set; }

        public string Owner { get; internal set; }

        public int CodeCount { get; internal set; }
    }
}