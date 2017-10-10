namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.ElasticSearch.Models;

    using Nest;

    using Serilog;

    internal class IndexKeeper : IIndexKeeper
    {
        private readonly ILogger logger;

        private readonly ElasticClient client;

        public IndexKeeper(ILogger logger, ElasticClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        public bool IndexExists(string indexName)
        {
            var descriptor = new IndexExistsDescriptor(indexName);
            var response = this.client.IndexExists(descriptor);
            if (!response.IsValid)
            {
            }

            return response.Exists;
        }

        public void CreateIndex(string indexName, CreateIndexDescriptor descriptor)
        {
            this.client.CreateIndex(indexName, d => descriptor);
        }

        public void DropIndex(string indexName)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<string> GetIndexesForAlias(string alias)
        {
            throw new System.NotImplementedException();
        }

        public void ReassignAlias(string oldIndexName, string newIndexName, string alias)
        {
            throw new System.NotImplementedException();
        }
    }
}