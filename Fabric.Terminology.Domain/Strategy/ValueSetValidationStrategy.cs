namespace Fabric.Terminology.Domain.Strategy
{
    using System;

    using Fabric.Terminology.Domain.Models;

    public class ValueSetValidationStrategy : IValueSetValidationStrategy
    {
        public bool EnsureIsCustom(IValueSet valueSet)
        {
            // TODO - relates to GH issue # 13
            return Guid.TryParse(valueSet.ValueSetId, out Guid empty) && empty.Equals(Guid.Empty);
        }
    }
}