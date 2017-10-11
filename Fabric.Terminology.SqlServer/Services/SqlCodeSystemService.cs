namespace Fabric.Terminology.SqlServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Persistence;

    internal class SqlCodeSystemService : ICodeSystemService
    {
        private readonly ICodeSystemRepository codeSystemRepository;

        public SqlCodeSystemService(ICodeSystemRepository codeSystemRepository)
        {
            this.codeSystemRepository = codeSystemRepository;
        }

        public Maybe<ICodeSystem> GetCodeSystem(Guid codeSystemGuid)
        {
            return this.codeSystemRepository.GetCodeSystem(codeSystemGuid);
        }

        public IReadOnlyCollection<ICodeSystem> GetAll(params Guid[] codeSystemGuids)
        {
            return this.codeSystemRepository.GetAll(codeSystemGuids);
        }
    }
}
