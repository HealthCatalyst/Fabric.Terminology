namespace Fabric.Terminology.Domain.Strategy
{
    using System;

    using Fabric.Terminology.Domain.Models;

    public class IdentifyIsCustomStrategy : IIdentifyIsCustomStrategy
    {
        public bool Execute(IValueSet valueSet)
        {
            // TODO - relates to GH issue # 13
            return Guid.TryParse(valueSet.ValueSetUniqueId, out Guid _);
        }

        public void SetIsCustom(IValueSet valueSet)
        {
            ((ValueSet)valueSet).IsCustom = this.Execute(valueSet);
        }
    }
}