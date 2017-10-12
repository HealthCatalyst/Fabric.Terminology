namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Factories;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;

    using Serilog;

    internal class SqlCodeSystemCodeRepository : ICodeSystemCodeRepository
    {
        private readonly SharedContext sharedContext;

        private readonly ILogger logger;

        private readonly ICodeSystemCodeCachingManager codeSystemCodeCachingManager;

        public SqlCodeSystemCodeRepository(
            SharedContext sharedContext,
            ILogger logger,
            ICodeSystemCodeCachingManager codeSystemCodeCachingManager)
        {
            this.sharedContext = sharedContext;
            this.logger = logger;
            this.codeSystemCodeCachingManager = codeSystemCodeCachingManager;
        }

        public Maybe<ICodeSystemCode> GetCodeSystemCode(Guid codeGuid)
        {
            try
            {
                return Maybe.From(this.codeSystemCodeCachingManager.GetOrSet(codeGuid, this.QueryCodeSystemCode));
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"Failed to get CodeSystemCode with Guid: {codeGuid}");
                throw;
            }
        }

        public Task<IReadOnlyCollection<ICodeSystemCode>> GetCodeSystemCodesListAsync(IEnumerable<Guid> codeGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetCodeSystemCodesAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            bool includeRetired)
        {
            throw new NotImplementedException();
        }

        private IQueryable<CodeSystemCodeDto> GetBaseQuery()
        {
            return this.sharedContext.CodeSystemCodes.Include(codeDto => codeDto.CodeSystem);
        }

        private ICodeSystemCode QueryCodeSystemCode(Guid codeGuid)
        {
            var factory = new CodeSystemCodeFactory();
            var dto = this.GetBaseQuery().SingleOrDefault(d => d.CodeGUID == codeGuid);
            return dto != null ? factory.Build(dto) : null;
        }
    }
}
