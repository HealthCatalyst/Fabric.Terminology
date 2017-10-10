namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.ElasticSearch.Models;

    using Nest;

    using Serilog;

    internal class ValueSetIndexer : IValueSetIndexer
    {
        private const string IndexAlias = "valuesets";

        private readonly ILogger logger;

        private readonly ElasticClient client;

        public ValueSetIndexer(ILogger logger, ElasticClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        public void CreateIndex(string indexName)
        {
            this.DropIndex(indexName);

            this.client.CreateIndex(
                indexName,
                c => c.Mappings(
                    m => m.Map<ValueSetIndexModel>(t => t.AutoMap())
                        .Map<ValueSetCodeIndexModel>(t => t.AutoMap())
                        .Map<ValueSetCodeCountIndexModel>(t => t.AutoMap())
                ));
        }

        public void DropIndex(string indexName)
        {
            var request = new IndexExistsRequest(indexName);
            var result = this.client.IndexExists(request);
            if (result.Exists)
            {
                this.client.DeleteIndex(indexName);
            }
        }

        public IReadOnlyCollection<string> GetIndexesForAlias(string alias = IndexAlias)
        {
            return this.client.GetIndicesPointingToAlias(alias).ToList();
        }

        public void SetAlias(string indexName, string alias = IndexAlias)
        {
            this.client.Alias(d => d.Add(a => a.Alias(alias).Index(indexName)));
        }

        public void IndexSingle(ValueSetIndexModel model, string alias = IndexAlias)
        {
            this.client.Index(model, descriptor => descriptor.Index(alias));
        }

        public void IndexMany(IEnumerable<ValueSetIndexModel> models)
        {
            var indexResponse = this.client.Bulk(
                s => s.IndexMany(
                    models,
                    (bulkDescriptor, record) => bulkDescriptor.Index(IndexAlias).Document(record)));

            if (!indexResponse.IsValid)
            {
                this.logger.Error(indexResponse.OriginalException, indexResponse.DebugInformation);
            }
        }

        public void RemoveSingle(ValueSetIndexModel model)
        {
            this.RemoveSingle(model.ValueSetGuid);
        }

        public void RemoveSingle(Guid valueSetGuid, string alias = IndexAlias)
        {
            this.client.Delete<ValueSetIndexModel>(valueSetGuid, descriptor => descriptor.Index(alias));
        }
    }
}