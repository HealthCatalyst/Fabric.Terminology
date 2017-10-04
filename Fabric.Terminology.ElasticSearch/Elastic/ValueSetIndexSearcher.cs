namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.ElasticSearch.Models;

    using Nest;

    using Serilog;

    internal class ValueSetIndexSearcher : IValueSetIndexSearcher
    {
        private const string IndexName = "valuesets";

        private readonly ILogger logger;

        private readonly ElasticClient client;

        public ValueSetIndexSearcher(ILogger logger, ElasticClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        public Maybe<ValueSetIndexModel> Get(Guid valueSetGuid)
        {
            var response = this.client.Get<ValueSetIndexModel>(
                valueSetGuid,
                descriptor => descriptor.Index(IndexName));
            return Maybe.If(response.IsValid, response.Source);
        }

        public Maybe<ValueSetCodeIndexModel> Get(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ValueSetIndexModel> GetMultiple(IEnumerable<Guid> valueSetGuids, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ValueSetIndexModel> GetVersions(string valueSetReferenceId)
        {
            var response = this.client.Search<ValueSetIndexModel>(
                g => g.Index(IndexName)
                    .From(0)
                    .Size(int.MaxValue)
                    .Query(q => q.Term(p => p.Name("valueSetReferenceId").Value(valueSetReferenceId))));

            return response.IsValid ? response.Documents : new List<ValueSetIndexModel>();
        }

        public PagedCollection<ValueSetIndexModel> GetPaged(IPagerSettings settings, bool latestVersionsOnly = true)
        {
            throw new NotImplementedException();
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
    }
}