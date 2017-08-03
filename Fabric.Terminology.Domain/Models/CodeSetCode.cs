namespace Fabric.Terminology.Domain.Models
{
    using System;

    public class CodeSetCode : ICodeSetCode
    {
        public CodeSetCode(
            string code,
            string name,
            ICodeSystem codeSystem,
            string versionDescription,
            string sourceDescription,
            DateTime lastLoadDate,
            DateTime? revisionDate)
        {
            this.Code = code;
            this.Name = name;
            this.CodeSystem = codeSystem;
            this.VersionDescription = versionDescription;
            this.SourceDescription = sourceDescription;
            this.LastLoadDate = lastLoadDate;
            this.RevisionDate = revisionDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeSetCode"/> class.
        /// </summary>
        /// <remarks>
        /// Prevents public construction
        /// Used for testing
        /// </remarks>
        internal CodeSetCode()
        {
        }

        public string Code { get; internal set; }

        public string Name { get; internal set; }

        public string VersionDescription { get; internal set; }

        public string SourceDescription { get; internal set; }

        public DateTime? RevisionDate { get; internal set; }

        public ICodeSystem CodeSystem { get; internal set; }

        public DateTime LastLoadDate { get; internal set; }
    }
}