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

    using Nest;

    using Serilog;

    public class ElasticValueSetSummaryService : IValueSetSummaryService
    {
        private readonly IValueSetIndexSearcher searcher;

        public ElasticValueSetSummaryService(IValueSetIndexSearcher searcher)
        {
            this.searcher = searcher;
        }

        public Maybe<IValueSetSummary> GetValueSetSummary(Guid valueSetGuid)
        {
            return this.searcher.Get(valueSetGuid)
                .Select(model => new ValueSetSummary(model, model.CodeCounts) as IValueSetSummary);
        }

        public Maybe<IValueSetSummary> GetValueSetSummary(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IValueSetSummary>> GetValueSetSummaries(IEnumerable<Guid> valueSetGuids)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IValueSetSummary>> GetValueSetSummaries(IEnumerable<Guid> valueSetGuids, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IValueSetSummary>> GetValueSetVersions(string valueSetReferenceId)
        {
            return Task.FromResult((IReadOnlyCollection<IValueSetSummary>)
                this.searcher.GetVersions(valueSetReferenceId)
                .Select(vim => Maybe.From(new ValueSetSummary(vim, vim.CodeCounts) as IValueSetSummary))
                .Values());
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(IPagerSettings settings, bool latestVersionsOnly = true)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(IPagerSettings settings, IEnumerable<Guid> codeSystemGuids, bool latestVersionsOnly = true)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(string nameFilterText, IPagerSettings pagerSettings, bool latestVersionsOnly = true)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true)
        {
            throw new NotImplementedException();
        }
    }
}
