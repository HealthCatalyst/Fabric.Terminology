namespace Fabric.Terminology.API.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    public class ValueSetCodeApiModel
    {
        public string Code { get; set; }

        public string ValueSetUniqueId { get; set; }

        public string ValueSetOId { get; set; }

        public string ValueSetId { get; set; }

        public string Name { get; set; }

        public CodeSystem CodeSystem { get; set; }

        public string SourceDescription { get; set; }

        public DateTime LastLoadDate { get; set; }
    }
}