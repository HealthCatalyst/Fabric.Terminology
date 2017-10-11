namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Factories;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlCodeSystemRepository : ICodeSystemRepository
    {
        private readonly SharedContext sharedContext;

        private readonly ILogger logger;

        public SqlCodeSystemRepository(SharedContext sharedContext, ILogger logger)
        {
            this.sharedContext = sharedContext;
            this.logger = logger;
        }

        public Maybe<ICodeSystem> GetCodeSystem(Guid codeSystemGuid)
        {
            var factory = new CodeSystemFactory();

            try
            {
                return Maybe
                    .From(this.sharedContext.CodeSystems.SingleOrDefault(x => x.CodeSystemGuid == codeSystemGuid))
                    .Select(d => factory.Build(d));
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"Failed to GetCodeSystem with Guid: {codeSystemGuid}");
                throw;
            }

        }

        public IReadOnlyCollection<ICodeSystem> GetAll(params Guid[] codeSystemGuids)
        {
            var factory = new CodeSystemFactory();

            try
            {
                var dtos = this.sharedContext.CodeSystems.AsNoTracking();

                if (codeSystemGuids.Any())
                {
                    dtos = dtos.Where(dto => codeSystemGuids.Contains(dto.CodeSystemGuid));
                }

                return dtos.OrderBy(dto => dto.CodeSystemNM).Select(factory.Build).ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"Failed to GetAll CodeSystems");
                throw;
            }
        }
    }
}