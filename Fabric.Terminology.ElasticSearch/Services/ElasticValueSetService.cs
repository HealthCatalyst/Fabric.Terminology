namespace Fabric.Terminology.ElasticSearch.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.ElasticSearch.Elastic;
    using Fabric.Terminology.ElasticSearch.Models;

    public class ElasticValueSetService : IValueSetService
    {
        private readonly IValueSetIndexSearcher searcher;

        public ElasticValueSetService(IValueSetIndexSearcher searcher)
        {
            this.searcher = searcher;
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid)
        {
            return this.searcher.Get(valueSetGuid).Select(Map);
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids)
        {
            return this.GetValueSet(valueSetGuid)
                .Select(
                    vs =>
                        {
                            var systemGuids = codeSystemGuids as Guid[] ?? codeSystemGuids.ToArray();
                            return systemGuids.Any()
                                       ? vs.CodeCounts.Any(cc => systemGuids.Contains(cc.CodeSystemGuid))
                                             ? vs
                                             : null
                                       : vs;
                        });
        }

        public Task<IReadOnlyCollection<IValueSet>> GetValueSets(IEnumerable<Guid> valueSetGuids)
        {
            return Task.FromResult((IReadOnlyCollection<IValueSet>)this.searcher.GetMultiple(valueSetGuids).Select(Map).ToList());
        }

        public Task<IReadOnlyCollection<IValueSet>> GetValueSets(IEnumerable<Guid> valueSetGuids, IEnumerable<Guid> codeSystemGuids)
        {
            var systemGuids = codeSystemGuids as Guid[] ?? codeSystemGuids.ToArray();
            if (!systemGuids.Any())
            {
                return this.GetValueSets(valueSetGuids);
            }

            return Task.FromResult(
                (IReadOnlyCollection<IValueSet>)this.searcher.GetMultiple(valueSetGuids)
                    .Where(vs => vs.CodeCounts.Any(cc => systemGuids.Contains(cc.CodeSystemGuid)))
                    .Select(Map).ToList());
        }

        public Task<IReadOnlyCollection<IValueSet>> GetValueSetVersions(string valueSetReferenceId)
        {
            var results = this.searcher.GetVersions(valueSetReferenceId).Select(Map).ToList();

            return Task.FromResult((IReadOnlyCollection<IValueSet>)results);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings settings,
            bool latestVersionsOnly = true)
        {
            return Task.FromResult(Map(this.searcher.GetPaged(settings, latestVersionsOnly)));
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true)
        {
            return Task.FromResult(Map(this.searcher.GetPaged(settings, codeSystemGuids, latestVersionsOnly)));
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            string nameFilterText,
            IPagerSettings settings,
            bool latestVersionsOnly = true)
        {
            return Task.FromResult(Map(this.searcher.GetPaged(nameFilterText, settings, latestVersionsOnly)));
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            string nameFilterText,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true)
        {
            return Task.FromResult(Map(this.searcher.GetPaged(nameFilterText, settings, codeSystemGuids, latestVersionsOnly)));
        }

        private static PagedCollection<IValueSet> Map(PagedCollection<ValueSetIndexModel> ip)
        {
            return new PagedCollection<IValueSet>
            {
                TotalPages = ip.TotalPages,
                TotalItems = ip.TotalItems,
                PagerSettings = ip.PagerSettings,
                Values = ip.Values.Select(Map).ToList()
            };
        }

        private static IValueSet Map(ValueSetIndexModel model)
        {
            return new ValueSet(model, model.ValueSetCodes, model.CodeCounts);
        }
    }
}
