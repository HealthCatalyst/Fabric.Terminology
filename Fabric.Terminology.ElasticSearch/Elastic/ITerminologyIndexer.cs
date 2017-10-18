namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.ElasticSearch.Models;

    using Nest;

    public interface ITerminologyIndexer
    {
        bool IndexSingle<T>(T model, string indexName)
            where T : class, IIndexModel;

        bool IndexMany<T>(IEnumerable<T> models, string indexName)
            where T : class, IIndexModel;

        bool RemoveSingle<T>(T model, string indexName)
            where T : class, IIndexModel;

        bool RemoveSingle<T>(Guid id, string indexName)
            where T : class, IIndexModel;

        string GetNameForIndexByConvention(string alias);

        bool IndexExists(string indexName);

        bool CreateIndex(string indexName, CreateIndexDescriptor descriptor);

        bool DropIndex(string indexName);

        IReadOnlyCollection<string> GetIndexesForAlias(string alias);

        bool ReassignAlias(string newIndexName, string alias);
    }
}