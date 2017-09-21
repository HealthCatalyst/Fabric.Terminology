namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    internal class ValueSet : ValueSetBase, IValueSet
    {
        internal ValueSet()
        {
        }

        internal ValueSet(IValueSetBackingItem backingItem)
            : base(backingItem)
        {
        }

        public IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; internal set; } = new List<IValueSetCode>();
    }
}