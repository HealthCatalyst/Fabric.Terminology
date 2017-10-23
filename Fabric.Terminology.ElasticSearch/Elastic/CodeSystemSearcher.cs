namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            return this.GetAll(false, codeSystemGuids);
        }

        public IReadOnlyCollection<CodeSystemIndexModel> GetAll(bool includeZeroCountCodeSystems, params Guid[] codeSystemGuids)
        {
            var descriptor = new SearchDescriptor<CodeSystemIndexModel>().Index(IndexAlias).From(0).Size(Constants.NestMaxPageSize);

            if (codeSystemGuids.Any())
            {
                descriptor = descriptor.Query(q => q.Terms(f => f.Field("codeSystemGuid").Terms(codeSystemGuids)));
            }

            if (!includeZeroCountCodeSystems)
            {
                descriptor = descriptor.Query(q => q.Range(r => r.Field("codeCount").GreaterThan(0)));
            }

            var response = this.client.Search<CodeSystemIndexModel>(descriptor);

            return this.Map(response).OrderBy(cs => cs.Name).ToList();
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