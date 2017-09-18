namespace Fabric.Terminology.Domain.Models
{
    using System;

    public class CodeSetCode : ICodeSetCode
    {
        public CodeSetCode(
            Guid codeGuid,
            string code,
            string name,
            string codeSystemCode)
        {
            this.CodeGuid = codeGuid;
            this.Code = code;
            this.Name = name;
            this.CodeSystemCode = codeSystemCode;
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

        public Guid CodeGuid { get; internal set; }

        public string Code { get; internal set; }

        public string Name { get; internal set; }

        public string CodeSystemCode { get; internal set; }
    }
}