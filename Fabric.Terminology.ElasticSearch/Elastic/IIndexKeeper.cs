namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System.Collections.Generic;

    using Nest;

    public interface IIndexKeeper
    {
        bool IndexExists(string indexName);

        void CreateIndex(string indexName, CreateIndexDescriptor descriptor);

        void DropIndex(string indexName);

        IReadOnlyCollection<string> GetIndexesForAlias(string alias);

        void ReassignAlias(string oldIndexName, string newIndexName, string alias);
    }
}