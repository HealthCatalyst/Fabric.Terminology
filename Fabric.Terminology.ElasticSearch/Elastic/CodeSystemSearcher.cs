namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.ElasticSearch.Models;

    using Nest;

    using Serilog;

    internal class CodeSystemSearcher : ICodeSystemSearcher
    {
        private const string IndexAlias = Constants.CodeSystemIndexAlias;

        private readonly ILogger logger;

        private readonly ElasticClient client;

        public CodeSystemSearcher(ILogger logger, ElasticClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        public Maybe<CodeSystemIndexModel> Get(Guid codeSystemGuid)
        {
            var response = this.client.Get<CodeSystemIndexModel>(
                codeSystemGuid,
                descriptor => descriptor.Index(IndexAlias));

            if (!response.IsValid)
            {
                this.logger.Error(response.OriginalException, response.DebugInformation);
            }

            return Maybe.If(response.IsValid, response.Source);
        }

        public IReadOnlyCollection<CodeSystemIndexModel> GetAll(params Guid[] codeSystemGuids)
        {
            var response = this.client.Search<CodeSystemIndexModel>(
                g => g.Index(IndexAlias)
                .From(0)
                .Size(int.MaxValue)
                .Query(q => q.Terms(p => p.Field("codeSystemGuid").Terms(codeSystemGuids))));

            return this.Map(response);
        }

        private IReadOnlyCollection<CodeSystemIndexModel> Map(
            ISearchResponse<CodeSystemIndexModel> response)
        {
            if (!response.IsValid)
            {
                this.logger.Error(response.OriginalException, response.DebugInformation);
                return new List<CodeSystemIndexModel>();
            }

            return response.Documents;
        }
    }
}