namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.ElasticSearch.Models;

    public interface ICodeSystemSearcher
    {
        Maybe<CodeSystemIndexModel> Get(Guid codeSystemGuid);

        IReadOnlyCollection<CodeSystemIndexModel> GetAll(params Guid[] codeSystemGuids);

        IReadOnlyCollection<CodeSystemIndexModel> GetAll(
            bool includeZeroCountCodeSystems,
            params Guid[] codeSystemGuids);
    }
}
