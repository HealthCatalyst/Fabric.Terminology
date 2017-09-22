namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    internal class ValueSet : ValueSetBase, IValueSet
    {
        internal ValueSet()
        {
            this.ValueSetCodes = new List<IValueSetCode>();
        }

        internal ValueSet(IValueSetBackingItem backingItem, IReadOnlyCollection<IValueSetCode> codes)
            : base(backingItem)
        {
            this.ValueSetCodes = codes;
        }

        public IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; internal set; }
    }
}