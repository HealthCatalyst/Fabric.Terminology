namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Factories;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlCodeSystemRepository : ICodeSystemRepository
    {
        private readonly SharedContext sharedContext;

        private readonly ILogger logger;

        private readonly ICodeSystemCachingManager codeSystemCachingManager;

        public SqlCodeSystemRepository(
            SharedContext sharedContext,
            ILogger logger,
            ICodeSystemCachingManager codeSystemCachingManager)
        {
            this.sharedContext = sharedContext;
            this.logger = logger;
            this.codeSystemCachingManager = codeSystemCachingManager;
        }

        public Maybe<ICodeSystem> GetCodeSystem(Guid codeSystemGuid)
        {
            try
            {
                return Maybe.From(this.codeSystemCachingManager.GetOrSet(codeSystemGuid, this.QueryCodeSystem));
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"Failed to GetCodeSystem with Guid: {codeSystemGuid}");
                throw;
            }
        }

        public IReadOnlyCollection<ICodeSystem> GetAll(params Guid[] codeSystemGuids)
        {
            try
            {
                return this.codeSystemCachingManager.GetMultipleOrQuery(this.QueryAll, codeSystemGuids);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"Failed to GetAll CodeSystems");
                throw;
            }
        }

        private ICodeSystem QueryCodeSystem(Guid codeSystemGuid)
        {
            var factory = new CodeSystemFactory();
            var dto = this.sharedContext.CodeSystems.SingleOrDefault(d => d.CodeSystemGuid == codeSystemGuid);
            return dto != null ? factory.Build(dto) : null;
        }

        private IReadOnlyCollection<ICodeSystem> QueryAll(params Guid[] codeSystemGuids)
        {
            var factory = new CodeSystemFactory();

            var dtos = this.sharedContext.CodeSystems.AsNoTracking();

            // TODO inquire about the decision to only return code systems that have associated codes
            dtos = codeSystemGuids.Any() ?
                dtos.Where(dto => codeSystemGuids.Contains(dto.CodeSystemGuid)) :
                dtos.Where(dto => dto.CodeCountNBR > 0);

            return dtos.OrderBy(dto => dto.CodeSystemNM).Select(factory.Build).ToList();
        }
    }
}