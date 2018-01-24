namespace Fabric.Terminology.API.Models
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    public class ValueSetComparisonResultApiModel
    {
        public IEnumerable<ValueSetItemApiModel> Compared { get; set; }

        public int AggregateCodeCount { get; set; }

        public IEnumerable<CodeComparisonApiModel> CodeComparisons { get; set; }
    }
}