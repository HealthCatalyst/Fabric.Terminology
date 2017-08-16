namespace Fabric.Terminology.Domain
{
    using System;
    using System.Linq;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;

    /// <summary>
    /// Extension methods for <see cref="IValueSet"/>
    /// </summary>
    public static partial class Extensions
    {
        public static void SetIdsForCustomInsert(this IValueSet valueSet)
        {
            var sequentialGuid = GuidComb.GenerateComb().ToString();

            if (valueSet is ValueSet realValueSet)
            {
                realValueSet.ValueSetUniqueId = sequentialGuid;
                realValueSet.ValueSetId = sequentialGuid;
                realValueSet.ValueSetOId = sequentialGuid;
            }
            else
            {
                throw new NotImplementedException("IValueSet must be instantiated as ValueSet in this implementation.");
            }

            foreach (var code in valueSet.ValueSetCodes.Cast<ValueSetCode>())
            {
                code.ValueSetUniqueId = sequentialGuid;
                code.ValueSetId = sequentialGuid;
                code.ValueSetOId = sequentialGuid;
                code.ValueSetName = valueSet.Name;
            }
        }

        internal static bool IsNew(this IValueSet valueSet)
        {
            return Guid.TryParse(valueSet.ValueSetUniqueId, out Guid empty) && empty.Equals(Guid.Empty);
        }
    }
}