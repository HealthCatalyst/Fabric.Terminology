namespace Fabric.Terminology.ElasticSearch.Services
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.ElasticSearch.Elastic;

    internal class ElasticCodeSystemService : ICodeSystemService
    {
        private readonly ICodeSystemSearcher codeSystemSearcher;

        public ElasticCodeSystemService(ICodeSystemSearcher codeSystemSearcher)
        {
            this.codeSystemSearcher = codeSystemSearcher;
        }

        public Maybe<ICodeSystem> GetCodeSystem(Guid codeSystemGuid)
        {
            return this.codeSystemSearcher.Get(codeSystemGuid).Select(cs => cs as ICodeSystem);
        }

        public IReadOnlyCollection<ICodeSystem> GetAll(params Guid[] codeSystemGuids)
        {
            return this.codeSystemSearcher.GetAll(codeSystemGuids);
        }
    }
}
