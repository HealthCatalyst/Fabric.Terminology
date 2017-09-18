namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Services;

    public class ValueSet : ValueSetSummary, IValueSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueSet"/> class.
        /// </summary>
        /// <remarks>
        /// Prevents public construction to force creation via <see cref="IValueSetService"/>
        /// Used for testing.
        /// </remarks>
        internal ValueSet()
        {
        }

        public IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; internal set; } = new List<IValueSetCode>();
    }
}