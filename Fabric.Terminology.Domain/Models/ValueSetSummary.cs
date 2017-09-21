namespace Fabric.Terminology.Domain.Models
{
    using System;
    using System.Collections.Generic;

    internal class ValueSetSummary : ValueSetBase, IValueSetSummary
    {
        internal ValueSetSummary()
        {
        }

        internal ValueSetSummary(IValueSetBackingItem backingItem)
            : base(backingItem)
        {
        }

        public IReadOnlyCollection<IValueSetCodeCount> CodeCounts { get; internal set; }
    }
}