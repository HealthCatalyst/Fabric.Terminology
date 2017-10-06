namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            var response = this.client.Get<ValueSetIndexModel>(
                valueSetGuid,
                descriptor => descriptor.Index(IndexAlias));

            if (!response.IsValid)
            {
                this.logger.Error(response.OriginalException, response.DebugInformation);
            }

            return Maybe.If(response.IsValid, response.Source);
        }

        public Maybe<ValueSetIndexModel> GetByName(string name)
        {
            var response = this.client.Search<ValueSetIndexModel>(
                g => g.Index(IndexAlias).Query(q => q.MatchPhrase(c => c.Field("name").Query(name))));

            var models = this.Map(response);
            return Maybe.From(models.FirstOrDefault());
        }

        public IReadOnlyCollection<ValueSetIndexModel> GetMultiple(IEnumerable<Guid> valueSetGuids)
        {
            var response = this.client.Search<ValueSetIndexModel>(
                g => g.Index(IndexAlias).Query(q => q.Ids(v => v.Values(valueSetGuids))));

            return this.Map(response);
        }

        public IReadOnlyCollection<ValueSetIndexModel> GetVersions(string valueSetReferenceId)
        {
            var response = this.client.Search<ValueSetIndexModel>(
                g => g.Index(IndexAlias)
                    .Query(q => q.Match(p => p.Field("valueSetReferenceId").Query(valueSetReferenceId))));

            return this.Map(response);
        }

        public PagedCollection<ValueSetIndexModel> GetPaged(IPagerSettings settings, bool latestVersionsOnly = true)
        {
            var response = latestVersionsOnly
                               ? this.client.Search<ValueSetIndexModel>(
                                   g => g.Index(IndexAlias)
                                       .FromPagerSettings(settings)
                                       .Query(q => q.Bool(m => m.Filter(n => n.Term("isLatestVersion", true))))
                                       .Sort(p => p.Field("name.keyword", SortOrder.Ascending)))
                               : this.client.Search<ValueSetIndexModel>(
                                   g => g.Index(IndexAlias)
                                       .FromPagerSettings(settings)
                                       .Sort(p => p.Field("name.keyword", SortOrder.Ascending)));

            return this.Map(settings, response);
        }

        public PagedCollection<ValueSetIndexModel> GetPaged(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true)
        {
            var response = latestVersionsOnly
                               ? this.client.Search<ValueSetIndexModel>(
                                   g => g.Index(IndexAlias)
                                       .FromPagerSettings(settings)
                                       .Query(
                                           q => q.Bool(
                                               m => m.Must(
                                                       t => t.Terms(
                                                           p => p.Field("codeCounts.codeSystemGuid")
                                                               .Terms(codeSystemGuids)))
                                                   .Filter(n => n.Term("isLatestVersion", true))))
                                       .Sort(p => p.Field("name.keyword", SortOrder.Ascending)))
                               : this.client.Search<ValueSetIndexModel>(
                                   g => g.Index(IndexAlias)
                                       .FromPagerSettings(settings)
                                       .Query(
                                           q => q.Terms(
                                               p => p.Field("codeCounts.codeSystemGuid").Terms(codeSystemGuids)))
                                       .Sort(p => p.Field("name.keyword", SortOrder.Ascending)));

            return this.Map(settings, response);
        }

        public PagedCollection<ValueSetIndexModel> GetPaged(
            string nameFilterText,
            IPagerSettings settings,
            bool latestVersionsOnly = true)
        {
            var response = latestVersionsOnly ?

                this.client.Search<ValueSetIndexModel>(
                g => g.Index(IndexAlias)
                    .FromPagerSettings(settings)
                    .Query(
                        q => q.Bool(
                            m => m.Must(
                                    t => t.MultiMatch(
                                        p => p.Query(nameFilterText)
                                            .Fields(f => f.Field("name").Field("valueSetReferenceId", 3.0))
                                            .Query(nameFilterText)))
                                .Filter(n => n.Term("isLatestVersion", true))))) :

                    this.client.Search<ValueSetIndexModel>(
                        g => g.Index(IndexAlias)
                            .FromPagerSettings(settings)
                            .Query(
                                q => q.Bool(
                                    m => m.Must(
                                            t => t.MultiMatch(
                                                p => p.Query(nameFilterText)
                                                    .Fields(f => f.Field("name").Field("valueSetReferenceId", 2.0))
                                                    .Query(nameFilterText))))));

            return this.Map(settings, response);
        }

        public PagedCollection<ValueSetIndexModel> GetPaged(
            string nameFilterText,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true)
        {
            var response = latestVersionsOnly ?
                this.client.Search<ValueSetIndexModel>(
                    g => g.Index(IndexAlias)
                        .FromPagerSettings(settings)
                        .Query(
                            q => q.Bool(
                                m => m.Must(
                                        t => t.Terms(
                                                p => p.Field("codeCounts.codeSystemGuid")
                                                    .Terms(codeSystemGuids))
                                            && t.MultiMatch(
                                                p => p.Query(nameFilterText)
                                                    .Fields(
                                                        f => f.Field("name")
                                                            .Field("valueSetReferenceId", 2.0))
                                                    .Query(nameFilterText)))
                                                    .Filter(n => n.Term("isLatestVersion", true))))) :

                    this.client.Search<ValueSetIndexModel>(
                        g => g.Index(IndexAlias)
                            .FromPagerSettings(settings)
                            .Query(
                                q => q.Bool(
                                    m => m.Must(
                                            t => t.Terms(
                                                    p => p.Field("codeCounts.codeSystemGuid")
                                                        .Terms(codeSystemGuids))
                                                && t.MultiMatch(
                                                    p => p.Query(nameFilterText)
                                                        .Fields(
                                                            f => f.Field("name")
                                                                .Field("valueSetReferenceId", 2.0))
                                                        .Query(nameFilterText)))
                                        .Filter(n => n.Term("isLatestVersion", true)))));
            return this.Map(settings, response);
        }

        private PagedCollection<ValueSetIndexModel> Map(
            IPagerSettings settings,
            ISearchResponse<ValueSetIndexModel> response)
        {
            if (!response.IsValid)
            {
                this.logger.Error(response.OriginalException, response.DebugInformation);
                return PagedCollection<ValueSetIndexModel>.Empty();
            }

            var strategy = this.pagingStrategyFactory.GetPagingStrategy<ValueSetIndexModel>(20);

            return strategy.CreatePagedCollection(response.Documents, (int)response.Total, settings);
        }

        private IReadOnlyCollection<ValueSetIndexModel> Map(
            ISearchResponse<ValueSetIndexModel> response)
        {
            if (!response.IsValid)
            {
                this.logger.Error(response.OriginalException, response.DebugInformation);
                return new List<ValueSetIndexModel>();
            }

            return response.Documents;
        }
    }
}