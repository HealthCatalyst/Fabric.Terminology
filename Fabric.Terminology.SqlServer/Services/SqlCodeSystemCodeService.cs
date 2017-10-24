namespace Fabric.Terminology.SqlServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public IReadOnlyCollection<ICodeSystemCode> GetCodeSystemCodes(IEnumerable<Guid> codeGuids)
        {
            return this.codeSystemCodeRepository.GetCodeSystemCodes(codeGuids);
        }

        public Task<ICsvQueryResult> GetCodeSystemCodesBatchAsync(IEnumerable<string> codes)
        {
            return this.GetCodeSystemCodesBatchAsync(codes, new List<Guid>());
        }

        public Task<ICsvQueryResult> GetCodeSystemCodesBatchAsync(IEnumerable<string> codes, IEnumerable<Guid> codeSystemGuids)
        {
            return this.codeSystemCodeRepository.GetCodeSystemCodesBatchAsync(codes, codeSystemGuids);
        }

        public Task<PagedCollection<ICodeSystemCode>> GetCodeSystemCodesAsync(IPagerSettings settings, bool includeRetired = false)
        {
            return this.GetCodeSystemCodesAsync(settings, Enumerable.Empty<Guid>(), includeRetired);
        }

        public Task<PagedCollection<ICodeSystemCode>> GetCodeSystemCodesAsync(
            IPagerSettings settings,
            Guid codeSystemGuid,
            bool includeRetired = false)
        {
            return this.GetCodeSystemCodesAsync(settings, new[] { codeSystemGuid }, includeRetired);
        }

        public Task<PagedCollection<ICodeSystemCode>> GetCodeSystemCodesAsync(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            bool includeRetired = false)
        {
            return this.GetCodeSystemCodesAsync(string.Empty, settings, codeSystemGuids,includeRetired);
        }

        public Task<PagedCollection<ICodeSystemCode>> GetCodeSystemCodesAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            bool includeRetired = false)
        {
            return this.codeSystemCodeRepository.GetCodeSystemCodesAsync(
                filterText,
                pagerSettings,
                codeSystemGuids,
                includeRetired);
        }
    }
}