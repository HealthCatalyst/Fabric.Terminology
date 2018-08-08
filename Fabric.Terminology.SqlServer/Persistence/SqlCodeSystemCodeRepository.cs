namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Querying;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Factories;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlCodeSystemCodeRepository : ICodeSystemCodeRepository
    {
        private readonly SharedContext sharedContext;

        private readonly ILogger logger;

        private readonly IPagingStrategyFactory pagingStrategyFactory;

        private readonly ICodeSystemCodeCachingManager codeSystemCodeCachingManager;

        private readonly Lazy<IReadOnlyCollection<ICodeSystem>> codeSystems;

        public SqlCodeSystemCodeRepository(
            SharedContext sharedContext,
            ILogger logger,
            ICodeSystemCodeCachingManager codeSystemCodeCachingManager,
            ICodeSystemRepository codeSystemRepository,
            IPagingStrategyFactory pagingStrategyFactory)
        {
            this.sharedContext = sharedContext;
            this.logger = logger;
            this.pagingStrategyFactory = pagingStrategyFactory;
            this.codeSystemCodeCachingManager = codeSystemCodeCachingManager;
            this.codeSystems = new Lazy<IReadOnlyCollection<ICodeSystem>>(() => codeSystemRepository.GetAll());
        }

        public Maybe<ICodeSystemCode> GetCodeSystemCode(Guid codeGuid)
        {
            try
            {
                return this.codeSystemCodeCachingManager.GetOrSet(codeGuid, this.QueryCodeSystemCode);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"Failed to get CodeSystemCode with Guid: {codeGuid}");
                throw;
            }
        }

        public IReadOnlyCollection<ICodeSystemCode> GetCodeSystemCodes(IEnumerable<Guid> codeGuids)
        {
            try
            {
                return this.codeSystemCodeCachingManager.GetMultipleOrQuery(
                    this.QueryCodeSystemCodeList,
                    true,
                    codeGuids.ToArray());
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"Failed to get a list CodeSystemCodes");
                throw;
            }
        }

        public async Task<IBatchCodeSystemCodeResult> GetCodeSystemCodesBatchAsync(
            IEnumerable<string> codes,
            IEnumerable<Guid> codeSystemGuids)
        {
            var codesHash = codes.ToHashSet();

            var found = new List<ICodeSystemCode>();
            var systemGuids = codeSystemGuids as Guid[] ?? codeSystemGuids.ToArray();

            foreach (var batch in codesHash.Batch(500))
            {
                var results = await this.GetCodesByBatch(batch.ToList(), systemGuids).ConfigureAwait(false);
                var unique = results.Where(r => !found.Exists(f => f.CodeGuid == r.CodeGuid));
                found.AddRange(unique);
            }

            return new BatchCodeSystemCodeResult
            {
                Matches = found,
                NotFound = codesHash.Where(c => !found.Exists(f => f.Code == c)).ToList()
            };
        }

        public Task<PagedCollection<ICodeSystemCode>> GetCodeSystemCodesAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            bool includeRetired)
        {
            var dtos = this.GetBaseQuery(includeRetired);

            if (!filterText.IsNullOrWhiteSpace())
            {
                dtos = dtos.Where(
                    c => c.CodeDSC.Contains(filterText, StringComparison.OrdinalIgnoreCase) ||
                         c.CodeCD.Equals(filterText, StringComparison.OrdinalIgnoreCase));
            }

            var systemGuids = codeSystemGuids as Guid[] ?? codeSystemGuids.ToArray();
            if (systemGuids.Any())
            {
                dtos = dtos.Where(c => systemGuids.Contains(c.CodeSystemGUID));
            }

            return this.CreatePagedCollectionAsync(dtos, pagerSettings);
        }

        private async Task<IReadOnlyCollection<ICodeSystemCode>> GetCodesByBatch(
            IReadOnlyCollection<string> codes,
            IReadOnlyCollection<Guid> codeSystemGuids)
        {
            var dtos = this.GetBaseQuery(true).Where(dto => codes.Contains(dto.CodeCD));
            if (codeSystemGuids.Any())
            {
                dtos = dtos.Where(dto => codeSystemGuids.Contains(dto.CodeSystemGUID));
            }

            var factory = new CodeSystemCodeFactory(this.codeSystems.Value);
            var results = await dtos.ToListAsync().ConfigureAwait(false);

            return results.Select(factory.Build).ToList();
        }

        private IQueryable<CodeSystemCodeDto> GetBaseQuery(bool includeRetired)
        {
            return includeRetired
                       ? this.sharedContext.CodeSystemCodes
                       : this.sharedContext.CodeSystemCodes
                           .Where(dto => dto.RetiredFLG == "N");
        }

        private ICodeSystemCode QueryCodeSystemCode(Guid codeGuid)
        {
            var factory = new CodeSystemCodeFactory(this.codeSystems.Value);
            var dto = this.GetBaseQuery(true).SingleOrDefault(d => d.CodeGUID == codeGuid);
            return dto != null ? factory.Build(dto) : null;
        }

        private IReadOnlyCollection<ICodeSystemCode> QueryCodeSystemCodeList(bool includeRetired, Guid[] codeGuids)
        {
            if (!codeGuids.Any())
            {
                return new List<ICodeSystemCode>();
            }

            var factory = new CodeSystemCodeFactory(this.codeSystems.Value);
            var dtos = this.GetBaseQuery(includeRetired).Where(dto => codeGuids.Contains(dto.CodeGUID));
            if (!includeRetired)
            {
                dtos = dtos.Where(dto => dto.RetiredFLG == "N");
            }

            return dtos.ToList().Select(factory.Build).ToList();
        }

        private async Task<PagedCollection<ICodeSystemCode>> CreatePagedCollectionAsync(
            IQueryable<CodeSystemCodeDto> source,
            IPagerSettings pagerSettings)
        {
            var defaultItemsPerPage = this.sharedContext.Settings.DefaultItemsPerPage;
            var pagingStrategy = this.pagingStrategyFactory.GetPagingStrategy<ICodeSystemCode>(defaultItemsPerPage);

            pagingStrategy.EnsurePagerSettings(pagerSettings);

            var count = await source.CountAsync().ConfigureAwait(false);
            var items = await source.OrderBy(dto => dto.CodeDSC)
                            .Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage)
                            .Take(pagerSettings.ItemsPerPage)
                            .ToListAsync().ConfigureAwait(false);

            var factory = new CodeSystemCodeFactory(this.codeSystems.Value);

            return pagingStrategy.CreatePagedCollection(
                items.Select(i => this.codeSystemCodeCachingManager.GetOrSet(i.CodeGUID, () => factory.Build(i))
                ).Values(),
                count,
                pagerSettings);
        }
    }
}
