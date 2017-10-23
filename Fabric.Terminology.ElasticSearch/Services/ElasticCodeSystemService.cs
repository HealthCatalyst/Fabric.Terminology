namespace Fabric.Terminology.ElasticSearch.Services
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    internal class ElasticCodeSystemService : ICodeSystemService
    {
        public Maybe<ICodeSystem> GetCodeSystem(Guid codeSystemGuid)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ICodeSystem> GetAll(params Guid[] codeSystemGuids)
        {
            throw new NotImplementedException();
        }
    }
}
