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
            return this.searcher.Get(valueSetGuid)
                .Select(model => new ValueSet(model, model.ValueSetCodes, model.CodeCounts) as IValueSet);
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IValueSet>> GetValueSets(IEnumerable<Guid> valueSetGuids)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IValueSet>> GetValueSets(IEnumerable<Guid> valueSetGuids, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IValueSet>> GetValueSetVersions(string valueSetReferenceId)
        {
            return Task.FromResult((IReadOnlyCollection<IValueSet>)
                this.searcher.GetVersions(valueSetReferenceId)
                    .Select(vim => Maybe.From(new ValueSet(vim, vim.ValueSetCodes, vim.CodeCounts) as IValueSet))
                    .Values());
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings, bool latestVersionsOnly = true)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings, IEnumerable<Guid> codeSystemGuids, bool latestVersionsOnly = true)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings, bool latestVersionsOnly = true)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true)
        {
            throw new NotImplementedException();
        }

        public bool NameIsUnique(string name)
        {
            throw new NotImplementedException();
        }

        public Attempt<IValueSet> Create(string name, IValueSetMeta meta, IEnumerable<ICodeSetCode> valueSetCodes)
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
    }
}
