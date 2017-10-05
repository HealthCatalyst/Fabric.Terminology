namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.ElasticSearch.Models;

    using Nest;

    using Serilog;

    internal class ValueSetIndexSearcher : IValueSetIndexSearcher
    {
        private const string IndexAlias = "valuesets";

        private readonly ElasticClient client;

        private readonly ILogger logger;

        private readonly IPagingStrategyFactory pagingStrategyFactory;

        public ValueSetIndexSearcher(ILogger logger, ElasticClient client, IPagingStrategyFactory pagingStrategyFactory)
        {
            this.logger = logger;
            this.client = client;
            this.pagingStrategyFactory = pagingStrategyFactory;
        }

        public Maybe<ValueSetIndexModel> Get(Guid valueSetGuid)
        {
            var response = this.client.Get<ValueSetIndexModel>(valueSetGuid, descriptor => descriptor.Index(IndexAlias));

            return Maybe.If(response.IsValid, response.Source);
        }

        public IReadOnlyCollection<ValueSetIndexModel> GetMultiple(IEnumerable<Guid> valueSetGuids)
        {
            var response = this.client.Search<ValueSetIndexModel>(
                g => g.Index(IndexAlias).Query(q => q.Ids(v => v.Values(valueSetGuids))));

            return response.IsValid ? response.Documents : new List<ValueSetIndexModel>();
        }

        public IReadOnlyCollection<ValueSetIndexModel> GetVersions(string valueSetReferenceId)
        {
            var response = this.client.Search<ValueSetIndexModel>(
                g => g.Index(IndexAlias)
                    .Query(q => q.Match(p => p.Field("valueSetReferenceId").Query(valueSetReferenceId))));

            return response.IsValid ? response.Documents : new List<ValueSetIndexModel>();
        }

        public PagedCollection<ValueSetIndexModel> GetPaged(IPagerSettings settings, bool latestVersionsOnly = true)
        {
            var response = latestVersionsOnly
                               ? this.client.Search<ValueSetIndexModel>(
                                   g => g.Index(IndexAlias)
                                       .From(settings.CurrentPage - 1)
                                       .Size(settings.ItemsPerPage)
                                       .Query(q => q.Bool(m => m.Filter(n => n.Term("isLatestVersion", true))))
                                       .Sort(p => p.Field("name.keyword", SortOrder.Ascending)))
                               : this.client.Search<ValueSetIndexModel>(
                                   g => g.Index(IndexAlias)
                                       .From(settings.CurrentPage - 1)
                                       .Size(settings.ItemsPerPage)
                                       .Sort(p => p.Field("name.keyword", SortOrder.Ascending)));

            return this.Map(settings, response);
        }

        public PagedCollection<ValueSetIndexModel> GetPaged(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true)
        {
            var response = latestVersionsOnly ?
                this.client.Search<ValueSetIndexModel>(
                    g => g.Index(IndexAlias)
                        .From(settings.CurrentPage - 1)
                        .Size(settings.ItemsPerPage)
                        .Query(q => q.Bool(m =>
                                m.Must(t => t.Terms(p => p.Field("codeCounts.codeSystemGuid").Terms(codeSystemGuids)))
                                .Filter(n => n.Term("isLatestVersion", true))
                                ))
                        .Sort(p => p.Field("name.keyword", SortOrder.Ascending)))
                :
                this.client.Search<ValueSetIndexModel>(
                g => g.Index(IndexAlias)
                    .From(settings.CurrentPage - 1)
                    .Size(settings.ItemsPerPage)
                    .Query(q => q.Terms(p => p.Field("codeCounts.codeSystemGuid").Terms(codeSystemGuids)))
                    .Sort(p => p.Field("name.keyword", SortOrder.Ascending)));

            return this.Map(settings, response);
        }

        public PagedCollection<ValueSetIndexModel> GetPaged(
            string nameFilterText,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true)
        {
            // TODO
            var response = this.client.Search<ValueSetIndexModel>(
                g => g.Index(IndexAlias)
                    .From(settings.CurrentPage - 1)
                    .Size(settings.ItemsPerPage)
                    .Query(
                        q => q.Bool(
                            m => m.Must(t => t.Terms(p => p.Field("codeCounts.codeSystemGuid").Terms(codeSystemGuids)) &&
                                             t.MatchPhrase(p => p.Field("name").Query(nameFilterText)))
                                .Filter(n => n.Term("isLatestVersion", true)))));

            return this.Map(settings, response);
        }

        private PagedCollection<ValueSetIndexModel> Map(
            IPagerSettings settings,
            ISearchResponse<ValueSetIndexModel> response)
        {
            if (!response.IsValid)
            {
                return PagedCollection<ValueSetIndexModel>.Empty();
            }

            var strategy = this.pagingStrategyFactory.GetPagingStrategy<ValueSetIndexModel>(20);

            return strategy.CreatePagedCollection(response.Documents, (int)response.Total, settings);
        }
    }
}