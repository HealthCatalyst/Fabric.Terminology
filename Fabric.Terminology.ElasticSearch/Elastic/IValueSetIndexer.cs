namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.ElasticSearch.Models;

    public interface IValueSetIndexer
    {
        void IndexSingle(ValueSetIndexModel model, string alias = Constants.IndexAlias);

        void IndexMany(IEnumerable<ValueSetIndexModel> models, string alias = Constants.IndexAlias);

        void RemoveSingle(ValueSetIndexModel model, string alias = Constants.IndexAlias);

        void RemoveSingle(Guid valueSetGuid, string alias = Constants.IndexAlias);
    }
}