namespace Fabric.Terminology.Domain.Models
{
    using System;

    public class ValueSetCode : CodeSetCode, IValueSetCode
    {
        public ValueSetCode(
            string valueSetId,
            string valueSetName,
            string code,
            string name,
            ICodeSystem codeSystem,
            string versionDescription,
            string sourceDescription,
            DateTime lastLoadDate,
            DateTime? revisionDate)
            : this(
                valueSetId,
                valueSetId,
                valueSetName,
                code,
                name,
                codeSystem,
                versionDescription,
                sourceDescription,
                lastLoadDate,
                revisionDate)
        {
        }

        public ValueSetCode(
            string valueSetId,
            string valueSetUniqueId,
            string valueSetName,
            string code,
            string name,
            ICodeSystem codeSystem,
            string versionDescription,
            string sourceDescription,
            DateTime lastLoadDate,
            DateTime? revisionDate)
            : this(
                valueSetId,
                valueSetUniqueId,
                valueSetUniqueId,
                valueSetName,
                code,
                name,
                codeSystem,
                versionDescription,
                sourceDescription,
                lastLoadDate,
                revisionDate)
        {
        }

        public ValueSetCode(
            string valueSetId,
            string valueSetUniqueId,
            string valueSetOId,
            string valueSetName,
            string code,
            string name,
            ICodeSystem codeSystem,
            string versionDescription,
            string sourceDescription,
            DateTime lastLoadDate,
            DateTime? revisionDate)
            : base(code, name, codeSystem, versionDescription, sourceDescription, lastLoadDate, revisionDate)
        {
            this.ValueSetId = valueSetId;
            this.ValueSetUniqueId = valueSetUniqueId;
            this.ValueSetOId = valueSetOId;
            this.ValueSetName = valueSetName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueSetCode"/> class.
        /// </summary>
        /// <remarks>
        /// Prevents public construction
        /// Used for testing
        /// </remarks>
        internal ValueSetCode()
        {
        }

        public string ValueSetUniqueId { get; internal set; }

        public string ValueSetOId { get; internal set; }

        public string ValueSetId { get; internal set; }

        public string ValueSetName { get; internal set; }
    }
}