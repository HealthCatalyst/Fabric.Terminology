namespace Fabric.Terminology.SqlServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Persistence;

    using Serilog;

    internal class SqlValueSetCodeService : IValueSetCodeService
    {
        private readonly ILogger logger;

        private readonly IValueSetCodeRepository valueSetCodeRepository;

        public SqlValueSetCodeService(ILogger logger, IValueSetCodeRepository valueSetCodeRepository)
        {
            this.logger = logger;
            this.valueSetCodeRepository = valueSetCodeRepository;
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodesByCodeGuid(Guid codeGuid)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(Guid valueSetGuid, IPagerSettings settings)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            Guid valueSetGuid,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(
            string filterText,
            Guid valueSetGuid,
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }
    }
}