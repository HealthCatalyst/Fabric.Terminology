namespace Fabric.Terminology.Domain
{
    using System;
    using System.Collections.Generic;
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
                realValueSet.ValueSetReferenceId = sequentialGuid.ToString();
            }
            else
            {
                throw new NotImplementedException("IValueSet must be instantiated as ValueSet in this implementation.");
            }

            foreach (var code in valueSet.ValueSetCodes.Cast<ValueSetCode>())
            {
                code.ValueSetGuid = sequentialGuid;
            }

            foreach (var count in valueSet.CodeCounts.Cast<ValueSetCodeCount>())
            {
                count.ValueSetGuid = sequentialGuid;
            }
        }

        internal static IReadOnlyCollection<IValueSetCodeCount> GetCodeCountsFromCodes(this IEnumerable<IValueSetCode> codes)
        {
            var valueSetCodes = codes as IValueSetCode[] ?? codes.ToArray();
            var codeSystems = valueSetCodes.Select(c => c.CodeSystemGuid).Distinct();
            return
                codeSystems.Select(codeSystemGuid => new { codeSystemGuid = codeSystemGuid, valueSetCode = valueSetCodes.First(c => c.CodeSystemGuid == codeSystemGuid) })
                .Select(
                    o => new ValueSetCodeCount
                    {
                        CodeSystemGuid = o.codeSystemGuid,
                        CodeCount = valueSetCodes.Count(c => c.CodeSystemGuid == o.codeSystemGuid),
                        ValueSetGuid = o.valueSetCode.ValueSetGuid,
                        CodeSystemName = o.valueSetCode.CodeSystemName
                    }).Cast<IValueSetCodeCount>().ToList();
        }
    }
}