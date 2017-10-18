namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.ElasticSearch.Models;

    using Nest;

    using Serilog;

    internal class TerminologyIndexer : ITerminologyIndexer
    {
        private readonly ILogger logger;

        private readonly ElasticClient client;

        public TerminologyIndexer(ILogger logger, ElasticClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        public bool IndexSingle<T>(T model, string indexName)
            where T : class, IIndexModel
        {
            var response = this.client.Index(model, descriptor => descriptor.Index(indexName));
            return response.IsValid;
        }

        public bool IndexMany<T>(IEnumerable<T> models, string indexName)
            where T : class, IIndexModel
        {
            var success = true;
            var indexModels = models as T[] ?? models.ToArray();
            foreach (var batch in indexModels.Batch(1000))
            {
                var indexResponse = this.client.Bulk(
                    s => s.IndexMany(
                        batch,
                        (bulkDescriptor, record) => bulkDescriptor.Index(indexName).Document(record)));

                if (!indexResponse.IsValid)
                {
                    this.logger.Error(indexResponse.OriginalException, indexResponse.DebugInformation);
                    success = false;
                }
            }

            return success;
        }

        public bool RemoveSingle<T>(T model, string indexName)
            where T : class, IIndexModel
        {
            return this.RemoveSingle<T>(Guid.Parse(model.Id), indexName);
        }

        public bool RemoveSingle<T>(Guid id, string indexName)
            where T : class, IIndexModel
        {
            var response = this.client.Delete<T>(id, descriptor => descriptor.Index(indexName));
            return response.IsValid;
        }

        public string GetNameForIndexByConvention(string alias)
        {
            if (alias.IsNullOrWhiteSpace())
            {
                var ex = new ArgumentException($"By convention, index names are based off aliases.  Alias may not be empty.");
                this.logger.Error(ex, "Could not create an index name.");
                throw ex;
            }

            return $"{alias.ToLower()}-{DateTime.Now:yyyyMMddHHmmss}";
        }

        public bool IndexExists(string indexName)
        {
            var descriptor = new IndexExistsDescriptor(indexName);
            var response = this.client.IndexExists(descriptor);
            if (!response.IsValid)
            {
                this.logger.Error(response.OriginalException, "Failed to determine if index exists");
                throw response.OriginalException;
            }

            return response.Exists;
        }

        public bool CreateIndex(string indexName, CreateIndexDescriptor descriptor)
        {
            var response = this.client.CreateIndex(indexName, d => descriptor);
            return response.IsValid;
        }

        public bool DropIndex(string indexName)
        {
            var request = new IndexExistsRequest(indexName);
            var result = this.client.IndexExists(request);
            if (result.Exists)
            {
                var response = this.client.DeleteIndex(indexName);
                return response.IsValid;
            }

            return false;
        }

        public IReadOnlyCollection<string> GetIndexesForAlias(string alias)
        {
            return this.client.GetIndicesPointingToAlias(alias).ToList();
        }

        public bool ReassignAlias(string newIndexName, string alias)
        {
            var existing = this.GetIndexesForAlias(alias);
            var descriptor = new BulkAliasDescriptor();
            descriptor.Add(r => r.Alias(alias).Index(newIndexName)).RemoveFromIndexes(alias, existing);
            var response = this.client.Alias(descriptor);
            return response.IsValid;
        }
    }
}