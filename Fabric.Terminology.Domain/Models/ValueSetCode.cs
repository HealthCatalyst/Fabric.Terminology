namespace Fabric.Terminology.Domain.Models
{
    using System;

    public class ValueSetCode : IValueSetCode
    {
        public string Code { get; set; }

        public string ValueSetId { get; set; }

        public string Name { get; set; }

        public string VersionDescription { get; set; }

        public DateTime? RevisionDate { get; set; }

        public IValueSetCodeSystem CodeSystem { get; set; }
    }
}