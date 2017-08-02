namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Services;

    public class ValueSet : IValueSet
    {
        public ValueSet(
            string valueSetId,
            string name,
            string authoringSourceDescription,
            string purposeDescription,
            string versionDescription,
            IReadOnlyCollection<IValueSetCode> valueSetCodes)
            : this(
                valueSetId,
                valueSetId,
                name,
                authoringSourceDescription,
                purposeDescription,
                versionDescription,
                valueSetCodes)
        {
        }

        public ValueSet(
            string valueSetId,
            string valueSetUniqueId,
            string name,
            string authoringSourceDescription,
            string purposeDescription,
            string versionDescription,
            IReadOnlyCollection<IValueSetCode> valueSetCodes)
            : this(
                valueSetId,
                valueSetUniqueId,
                valueSetUniqueId,
                name,
                authoringSourceDescription,
                purposeDescription,
                versionDescription,
                valueSetCodes)
        {
        }

        public ValueSet(
            string valueSetId,
            string valueSetUniqueId,
            string valueSetOId,
            string name,
            string authoringSourceDescription,
            string purposeDescription,
            string versionDescription,
            IReadOnlyCollection<IValueSetCode> valueSetCodes)
        {
            this.ValueSetId = valueSetId;
            this.ValueSetUniqueId = valueSetUniqueId;
            this.ValueSetOId = valueSetOId;
            this.Name = name;
            this.AuthoringSourceDescription = authoringSourceDescription;
            this.PurposeDescription = purposeDescription;
            this.VersionDescription = versionDescription;
            this.ValueSetCodes = valueSetCodes;
            this.IsCustom = true;
        }

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

        public string ValueSetUniqueId { get; internal set; }

        public string ValueSetId { get; internal set; }

        public string ValueSetOId { get; internal set; }

        public string Name { get; internal set; }

        public bool IsCustom { get; internal set; }

        public string AuthoringSourceDescription { get; internal set; }

        public string PurposeDescription { get; internal set; }

        public string SourceDescription { get; internal set; }

        public string VersionDescription { get; internal set; }

        public bool AllCodesLoaded => this.ValueSetCodesCount == this.ValueSetCodes.Count;

        /// <summary>
        /// Gets or sets the number (count) of codes in the value set.
        /// </summary>
        /// <remarks>
        /// Value is set, rather than calculated, so that API can return a summary object with the first X number of codes leaving 
        /// this count correct for representation in the UI.
        /// </remarks>
        public int ValueSetCodesCount { get; set; }

        public IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; internal set; } = new IValueSetCode[] { };
    }
}