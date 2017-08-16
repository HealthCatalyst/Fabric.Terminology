namespace Fabric.Terminology.Domain.Strategy
{
    using System;

    using Fabric.Terminology.Domain.Models;

    public class IsCustomValueStrategy : IIsCustomValueStrategy
    {
        public bool Get(IValueSet valueSet)
        {
            // TODO - relates to GH issue # 13
            return Guid.TryParse(valueSet.ValueSetId, out Guid _);
        }

        public void Set(IValueSet valueSet)
        {
            ((ValueSet)valueSet).IsCustom = this.Get(valueSet);
        }
    }
}