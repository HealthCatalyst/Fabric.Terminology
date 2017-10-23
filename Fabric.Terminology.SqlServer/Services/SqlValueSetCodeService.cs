namespace Fabric.Terminology.SqlServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Persistence;

    internal class SqlValueSetCodeService : IValueSetCodeService
    {
        private readonly IValueSetCodeRepository valueSetCodeRepository;

        public SqlValueSetCodeService(IValueSetCodeRepository valueSetCodeRepository)
        {
            this.valueSetCodeRepository = valueSetCodeRepository;
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid)
        {
            return this.valueSetCodeRepository.GetValueSetCodes(valueSetGuid);
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodesByCodeGuid(Guid codeGuid)
        {
            return this.valueSetCodeRepository.GetValueSetCodesByCodeGuid(codeGuid);
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(IPagerSettings settings)
        {
            return this.GetValueSetCodesAsync(settings, new List<Guid>());
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids)
        {
            return this.GetValueSetCodesAsync(string.Empty, settings, codeSystemGuids);
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(Guid valueSetGuid, IPagerSettings settings)
        {
            return this.GetValueSetCodesAsync(valueSetGuid, settings, new List<Guid>());
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            Guid valueSetGuid,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids)
        {
            return this.GetValueSetCodesAsync(string.Empty, valueSetGuid, settings, codeSystemGuids);
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            string filterText,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids)
        {
            return this.valueSetCodeRepository.GetValueSetCodesAsync(filterText, settings, codeSystemGuids);
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            string filterText,
            Guid valueSetGuid,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids)
        {
            return this.valueSetCodeRepository.GetValueSetCodesAsync(
                filterText,
                valueSetGuid,
                settings,
                codeSystemGuids);
        }
    }
}