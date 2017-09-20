namespace Fabric.Terminology.Domain.Models
{
    using System;

    public class CodeSetCode : ICodeSetCode
    {
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

        public Guid CodeGuid { get; internal set; }

        public Guid CodeSystemGuid { get; set; }

        public string Code { get; internal set; }

        public string Name { get; internal set; }
    }
}