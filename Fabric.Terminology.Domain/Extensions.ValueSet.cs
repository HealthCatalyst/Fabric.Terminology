namespace Fabric.Terminology.Domain
{
    using System;

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

            if (valueSet as ValueSet is null)
            {
                throw new NotImplementedException("IValueSet must be instantiated as ValueSet in this implementation.");
            }

            ((ValueSet)valueSet).ValueSetUniqueId = sequentialGuid;
            ((ValueSet)valueSet).ValueSetId = sequentialGuid;
            ((ValueSet)valueSet).ValueSetOId = sequentialGuid;

            foreach (var code in valueSet.ValueSetCodes)
            {
                ((ValueSetCode)code).ValueSetUniqueId = sequentialGuid;
                ((ValueSetCode)code).ValueSetId = sequentialGuid;
                ((ValueSetCode)code).ValueSetOId = sequentialGuid;
                ((ValueSetCode)code).ValueSetName = valueSet.Name;
            }
        }

        internal static bool IsNew(this IValueSet valueSet)
        {
            return Guid.TryParse(valueSet.ValueSetUniqueId, out Guid empty) && empty.Equals(Guid.Empty);
        }
    }
}