namespace Fabric.Terminology.Domain.Models
{
    using System;

    public class ValueSetCode : CodeSetCode, IValueSetCode
    {
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

        public Guid ValueSetGuid { get; internal set; }
    }
}