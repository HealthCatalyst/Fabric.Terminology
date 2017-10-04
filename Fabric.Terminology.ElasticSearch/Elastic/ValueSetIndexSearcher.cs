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
        private const string IndexName = "valuesets";

        private readonly ILogger logger;

        private readonly ElasticClient client;

        private readonly IPagingStrategyFactory pagingStrategyFactory;

        public ValueSetIndexSearcher(
            ILogger logger,
            ElasticClient client,
            IPagingStrategyFactory pagingStrategyFactory)
        {
            this.logger = logger;
            this.client = client;
            this.pagingStrategyFactory = pagingStrategyFactory;
        }

        public Maybe<ValueSetIndexModel> Get(Guid valueSetGuid)
        {
            var response = this.client.Get<ValueSetIndexModel>(
                valueSetGuid,
                descriptor => descriptor.Index(IndexName));

            return Maybe.If(response.IsValid, response.Source);
        }

        public IReadOnlyCollection<ValueSetIndexModel> GetMultiple(IEnumerable<Guid> valueSetGuids)
        {
            var response = this.client.Search<ValueSetIndexModel>(
                g => g.Index(IndexName).Query(q => q.Ids(v => v.Values(valueSetGuids))));

            return response.IsValid ? response.Documents : new List<ValueSetIndexModel>();
        }

        public IReadOnlyCollection<ValueSetIndexModel> GetVersions(string valueSetReferenceId)
        {
            var response = this.client.Search<ValueSetIndexModel>(
                g => g.Index(IndexName)
                    .Query(q => q.Match(p => p.Field("valueSetReferenceId").Query(valueSetReferenceId))));

            return response.IsValid ? response.Documents : new List<ValueSetIndexModel>();
        }

        public PagedCollection<ValueSetIndexModel> GetPaged(IPagerSettings settings, bool latestVersionsOnly = false)
        {
            var response = latestVersionsOnly ?
                this.client.Search<ValueSetIndexModel>(
                    g => g.Index(IndexName)
                    .From(settings.CurrentPage - 1)
                    .Size(settings.ItemsPerPage)
                    .Query(q => q.Bool(m => m.Must(n => n.MatchAll()).Filter(n => n.Term("latestVersionOnly", true))))
                    .Sort(p => p.Field("name", SortOrder.Ascending))) :

                this.client.Search<ValueSetIndexModel>(
                g => g.Index(IndexName)
                    .From(settings.CurrentPage - 1)
                    .Size(settings.ItemsPerPage)
                    .Query(q => q.MatchAll())
                    .Sort(p => p.Field("name", SortOrder.Ascending)));

            this.logger.Debug(response.ApiCall.DebugInformation);

            return this.Map(settings, response);
        }

        public PagedCollection<ValueSetIndexModel> GetPaged(IPagerSettings settings, IEnumerable<Guid> codeSystemGuids, bool latestVersionsOnly = true)
        {
            throw new NotImplementedException();
        }

        public PagedCollection<ValueSetIndexModel> GetPaged(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true)
        {
            throw new NotImplementedException();
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