﻿namespace Fabric.Terminology.SqlServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Persistence;

    internal class SqlCodeSystemCodeService : ICodeSystemCodeService
    {
        private readonly ICodeSystemCodeRepository codeSystemCodeRepository;

        public SqlCodeSystemCodeService(ICodeSystemCodeRepository codeSystemCodeRepository)
        {
            this.codeSystemCodeRepository = codeSystemCodeRepository;
        }

        public Maybe<ICodeSystemCode> GetCodeSystemCode(Guid codeGuid)
        {
            return this.codeSystemCodeRepository.GetCodeSystemCode(codeGuid);
        }

        public Task<IReadOnlyCollection<ICodeSystemCode>> GetCodeSystemCodesListAsync(IEnumerable<Guid> codeGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<ICodeSystemCode>> GetCodeSystemCodesAsync(IPagerSettings settings, bool includeRetired = false)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<ICodeSystemCode>> GetCodeSystemCodesAsync(IPagerSettings settings, IEnumerable<Guid> codeSystemGuids, bool includeRetired = false)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetCodeSystemCodesAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            bool includeRetired = false)
        {
            throw new NotImplementedException();
        }
    }
}