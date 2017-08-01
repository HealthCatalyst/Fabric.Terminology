namespace Fabric.Terminology.Domain.Models
{
    using Fabric.Terminology.Domain.Services;

    public class ValueSetCode : CodeSetCode, IValueSetCode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueSetCode"/> class.
        /// </summary>
        /// <remarks>
        /// Prevents public construction
        /// </remarks>
        internal ValueSetCode()
        {            
        }

        public string ValueSetUniqueId { get; set; }

        public string ValueSetOId { get; set; }

        public string ValueSetId { get; set; }

        public string ValueSetName { get; set; }
    }
}