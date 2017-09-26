namespace Fabric.Terminology.Domain.Models
{
    using System;
    using System.Collections.Generic;

    internal class ValueSetSummary : ValueSetBase, IValueSetSummary
    {
        internal ValueSetSummary()
        {
            this.CodeCounts = new List<IValueSetCodeCount>();
        }

        internal ValueSetSummary(IValueSetBackingItem backingItem, IReadOnlyCollection<IValueSetCodeCount> counts)
            : base(backingItem)
        {
            this.CodeCounts = counts;
        }

        public IReadOnlyCollection<IValueSetCodeCount> CodeCounts { get; internal set; }
    }
}