namespace Fabric.Terminology.API.Models
{
    using System;

    public class CodeSystemApiModel
    {
        public Guid CodeSystemGuid { get; set; }

        public string Name { get; set; }

        public DateTime VersionDate { get; set; }

        public string Description { get; set; }

        public string Copyright { get; set; }

        public string Owner { get; set; }
    }
}