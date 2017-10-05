namespace Fabric.Terminology.ElasticSearch.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.ElasticSearch.Elastic;
    using Fabric.Terminology.ElasticSearch.Models;

    using Nest;

    using Serilog;

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
                            return vs.CodeCounts.Any(cc => codeSystemGuids.Contains(cc.CodeSystemGuid)) ? vs : null;
                        });
        }

        public Task<IReadOnlyCollection<IValueSet>> GetValueSets(IEnumerable<Guid> valueSetGuids)
        {
            return Task.FromResult((IReadOnlyCollection<IValueSet>)this.searcher.GetMultiple(valueSetGuids).Select(Map));
        }

        public Task<IReadOnlyCollection<IValueSet>> GetValueSets(IEnumerable<Guid> valueSetGuids, IEnumerable<Guid> codeSystemGuids)
        {
            return Task.FromResult(
                (IReadOnlyCollection<IValueSet>)this.searcher.GetMultiple(valueSetGuids)
                    .Where(vs => vs.CodeCounts.Any(cc => codeSystemGuids.Contains(cc.CodeSystemGuid)))
                    .Select(Map));
        }

        public Task<IReadOnlyCollection<IValueSet>> GetValueSetVersions(string valueSetReferenceId)
        {
            var results = this.searcher.GetVersions(valueSetReferenceId).Select(Map).ToList();

            return Task.FromResult((IReadOnlyCollection<IValueSet>)results);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings, bool latestVersionsOnly = true)
        {
            return Task.FromResult(Map(this.searcher.GetPaged(settings, latestVersionsOnly)));
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings, IEnumerable<Guid> codeSystemGuids, bool latestVersionsOnly = true)
        {
            return Task.FromResult(Map(this.searcher.GetPaged(settings, codeSystemGuids, latestVersionsOnly)));
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(string nameFilterText, IPagerSettings settings, bool latestVersionsOnly = true)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            string nameFilterText,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true)
        {
            return Task.FromResult(Map(this.searcher.GetPaged(nameFilterText, settings, codeSystemGuids, latestVersionsOnly)));
        }

        public bool NameIsUnique(string name)
        {
            throw new NotImplementedException();
        }

        public Attempt<IValueSet> Create(string name, IValueSetMeta meta, IReadOnlyCollection<ICodeSetCode> codeSetCodes)
        {
            throw new NotImplementedException();
        }

        public void Save(IValueSet valueSet)
        {
            throw new NotImplementedException();
        }

        public void Delete(IValueSet valueSet)
        {
            throw new NotImplementedException();
        }

        public bool ValueSetGuidIsUnique(Guid valueSetGuid)
        {
            throw new NotImplementedException();
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
