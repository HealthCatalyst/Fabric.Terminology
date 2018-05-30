namespace Fabric.Terminology.API.Models
{
    using System;

    using JetBrains.Annotations;

    using NullGuard;

    public class CodeSystemApiModel
    {
        public Guid CodeSystemGuid { get; set; }

        public string Name { get; set; }

        public DateTime VersionDate { get; set; }

        public string Description { get; set; }

        public string Copyright { get; set; }

        public string Owner { get; set; }

        public int CodeCount { get; set; }
    }
}