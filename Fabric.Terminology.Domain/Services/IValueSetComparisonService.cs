namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetComparisonService
    {
        Task<ValueSetDiffComparisonResult> CompareValueSetCodes(IEnumerable<Guid> valueSetGuids);

        /// <summary>
        ///     Compares codes between value sets
        /// </summary>
        /// <param name="valueSetGuids">The ids of the ValueSets (ValueSetGUID) to be compared</param>
        /// <param name="codeSystemGuids">
        ///     The ids of the code systems used to limit the code systems to be considered in the comparison.  If empty, assumes all code
        ///     systems should be included in comparison.
        /// </param>
        /// <returns>Aggregate information about the comparison.</returns>
        Task<ValueSetDiffComparisonResult> CompareValueSetCodes(
            IEnumerable<Guid> valueSetGuids,
            IEnumerable<Guid> codeSystemGuids);
    }
}