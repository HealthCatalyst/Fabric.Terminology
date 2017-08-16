namespace Fabric.Terminology.API.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    public class CodeSetCodeApiModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public CodeSystem CodeSystem { get; set; }

        public string SourceDescription { get; set; }

        public string VersionDescription { get; set; }

        public DateTime LastLoadDate { get; set; }
    }
}