namespace Fabric.Terminology.Domain.Models
{
    using System;

    public class CodeSetCode : ICodeSetCode
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string VersionDescription { get; set; }

        public DateTime? RevisionDate { get; set; }

        public IValueSetCodeSystem CodeSystem { get; set; }
    }
}