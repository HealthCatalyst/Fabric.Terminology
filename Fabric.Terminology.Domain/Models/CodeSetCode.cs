namespace Fabric.Terminology.Domain.Models
{
    using System;

    public class CodeSetCode : ICodeSetCode
    {
        internal CodeSetCode()
        {            
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public string VersionDescription { get; set; }

        public string SourceDescription { get; set; }

        public DateTime? RevisionDate { get; set; }

        public ICodeSystem CodeSystem { get; set; }

        public DateTime LastLoadDate { get; set; }
    }
}