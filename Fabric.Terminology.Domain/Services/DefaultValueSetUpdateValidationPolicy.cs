namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Models;

    public class DefaultValueSetUpdateValidationPolicy : IValueSetUpdateValidationPolicy
    {
        private static readonly IDictionary<ValueSetStatus, IEnumerable<ValueSetStatus>> PolicyMap =
            new Dictionary<ValueSetStatus, IEnumerable<ValueSetStatus>>
        {
            { ValueSetStatus.Draft, new[] { ValueSetStatus.Active, ValueSetStatus.Archived } },
            { ValueSetStatus.Active, new[] { ValueSetStatus.Archived } },
            { ValueSetStatus.Archived, new[] { ValueSetStatus.Active } }
        };

        public bool CanBeDeleted(IValueSet valueSet)
        {
            return valueSet.IsCustom && valueSet.StatusCode == ValueSetStatus.Draft;
        }

        public bool CanChangeStatus(ValueSetStatus current, ValueSetStatus target)
        {
            if (!PolicyMap.ContainsKey(current))
            {
                throw new InvalidOperationException($"The status {current.ToString()} has not been mapped in {nameof(DefaultValueSetUpdateValidationPolicy)}");
            }

            return PolicyMap[current].Contains(target);
        }
    }
}