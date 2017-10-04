namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.ElasticSearch.Models;

    public interface IValueSetIndexSearcher
    {
        Maybe<ValueSetIndexModel> Get(Guid valueSetGuid);

        IReadOnlyCollection<ValueSetIndexModel> GetMultiple(IEnumerable<Guid> valueSetGuids);

        IReadOnlyCollection<ValueSetIndexModel> GetVersions(string valueSetReferenceId);

        PagedCollection<ValueSetIndexModel> GetPaged(IPagerSettings settings, bool latestVersionsOnly = true);

        PagedCollection<ValueSetIndexModel> GetPaged(IPagerSettings settings, IEnumerable<Guid> codeSystemGuids, bool latestVersionsOnly = true);

        PagedCollection<ValueSetIndexModel> GetPaged(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true);
    }
}