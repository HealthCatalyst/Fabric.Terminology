namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    internal class ValueSet : ValueSetBase, IValueSet
    {
        internal ValueSet()
        {
            this.ValueSetCodes = new List<IValueSetCode>();
        }

        internal ValueSet(IValueSetBackingItem backingItem, IReadOnlyCollection<IValueSetCode> codes, IReadOnlyCollection<IValueSetCodeCount> counts)
            : base(backingItem)
        {
            this.ValueSetCodes = codes;
            this.CodeCounts = counts;
        }

        public IReadOnlyCollection<IValueSetCodeCount> CodeCounts { get; }

        public IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; internal set; }
    }
}