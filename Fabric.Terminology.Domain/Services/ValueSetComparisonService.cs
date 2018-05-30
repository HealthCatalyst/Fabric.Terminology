namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fabric.Terminology.Domain.Models;

    public class ValueSetComparisonService : IValueSetComparisonService
    {
        private readonly IValueSetService valueSetService;

        public ValueSetComparisonService(
            IValueSetService valueSetService)
        {
            this.valueSetService = valueSetService;
        }

        public Task<ValueSetDiffComparisonResult> CompareValueSetCodes(IEnumerable<Guid> valueSetGuids)
        {
            return this.CompareValueSetCodes(valueSetGuids, new List<Guid>());
        }

        public async Task<ValueSetDiffComparisonResult> CompareValueSetCodes(
            IEnumerable<Guid> valueSetGuids,
            IEnumerable<Guid> codeSystemGuids)
        {
            var valueSetGuidHash = valueSetGuids.ToHashSet();
            var codeSysetmGuidHash = codeSystemGuids.ToHashSet();

            if (valueSetGuidHash.Count() < 2)
            {
                throw new ArgumentException($"Two or more ValueSetGUID expected. {valueSetGuidHash.Count()} passed.", nameof(valueSetGuids));
            }

            var valueSets = await this.valueSetService.GetValueSetsListAsync(valueSetGuidHash, codeSysetmGuidHash).ConfigureAwait(false);

            if (valueSets.Count != valueSetGuidHash.Count)
            {
                var notFound = string.Join(", ", valueSetGuidHash.Where(g => !valueSets.Select(vs => vs.ValueSetGuid).Contains(g)));

                throw new InvalidOperationException($"Could not find value sets with ValueSetGUID(s) {notFound}.  Comparison cannot be performed.");
            }

            var comparisons = this.GetValueSetCodeComparisons(valueSets, codeSysetmGuidHash);

            return new ValueSetDiffComparisonResult
                {
                    Compared = valueSets,
                    AggregateCodeCount = comparisons.Count(),
                    CodeComparisons = comparisons
                };
        }

        private IReadOnlyCollection<ValueSetCodeComparison> GetValueSetCodeComparisons(IEnumerable<IValueSet> valueSets, IReadOnlyCollection<Guid> codeSystemGuids)
        {
            var allCodes = valueSets.SelectMany(vs => vs.ValueSetCodes);
            if (codeSystemGuids.Any())
            {
                allCodes = allCodes.Where(vsc => codeSystemGuids.Contains(vsc.CodeSystemGuid));
            }

            var lookup = allCodes.ToLookup(vsc => vsc.CodeGuid);

            return lookup.Select(group => new ValueSetCodeComparison
                {
                    Code = (ICodeSystemCode)group.FirstOrDefault(),
                    ValueSetGuids = group.Select(vsc => vsc.ValueSetGuid)
                }).ToList();
        }
    }
}