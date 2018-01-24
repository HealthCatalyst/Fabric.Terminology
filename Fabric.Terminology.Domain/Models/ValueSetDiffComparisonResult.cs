namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    public class ValueSetDiffComparisonResult
    {
        public IEnumerable<IValueSetSummary> Compared { get; set; }

        public int AggregateCodeCount { get; set; }

        public IEnumerable<ValueSetCodeComparison> CodeComparisons { get; set; }
    }
}