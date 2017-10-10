namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.ElasticSearch.Models;

    public interface IValueSetIndexer
    {
        void CreateIndex(string indexName);

        void DropIndex(string indexName);

        IReadOnlyCollection<string> GetIndexesForAlias(string alias = "valuesets");

        void SetAlias(string indexName, string alias = "valuesets");

        void IndexSingle(ValueSetIndexModel model, string alias = "valuesets");

        void IndexMany(IEnumerable<ValueSetIndexModel> models, string alias = "valuesets");

        void RemoveSingle(ValueSetIndexModel model, string alias = "valuesets");

        void RemoveSingle(Guid valueSetGuid, string alias = "valuesets");
    }
}