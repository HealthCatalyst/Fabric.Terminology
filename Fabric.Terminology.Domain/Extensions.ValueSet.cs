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
            var sequentialGuid = GuidComb.GenerateComb();

            if (valueSet is ValueSet realValueSet)
            {
                realValueSet.ValueSetGuid = sequentialGuid;
            }
            else
            {
                throw new NotImplementedException("IValueSet must be instantiated as ValueSet in this implementation.");
            }

            foreach (var code in valueSet.ValueSetCodes.Cast<ValueSetCode>())
            {
                code.ValueSetGuid = sequentialGuid;
            }
        }

        public static bool AllCodesLoaded(this IValueSet valueSet)
        {
            return valueSet.CodeCounts.Sum(cc => cc.CodeCount) == valueSet.ValueSetCodes.Count;
        }

        internal static bool IsNew(this IValueSet valueSet)
        {
            return valueSet.ValueSetGuid.Equals(Guid.Empty);
        }
    }
}