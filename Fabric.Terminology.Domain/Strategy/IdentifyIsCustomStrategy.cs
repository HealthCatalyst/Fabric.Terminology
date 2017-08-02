namespace Fabric.Terminology.Domain.Strategy
{
    using System;

    using Fabric.Terminology.Domain.Models;

    public class IdentifyIsCustomStrategy : IIdentifyIsCustomStrategy
    {
        public bool Execute(IValueSet valueSet)
        {
            // TODO - relates to GH issue # 13
            return Guid.TryParse(valueSet.ValueSetId, out Guid empty) && empty.Equals(Guid.Empty);
        }

        public void SetIsCustom(IValueSet valueSet)
        {
            ((ValueSet)valueSet).IsCustom = this.Execute(valueSet);
        }
    }
}