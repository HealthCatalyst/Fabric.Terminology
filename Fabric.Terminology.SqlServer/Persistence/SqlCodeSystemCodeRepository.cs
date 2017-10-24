namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
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

        public SqlCodeSystemCodeRepository(
            SharedContext sharedContext,
            ILogger logger,
            ICodeSystemCodeCachingManager codeSystemCodeCachingManager,
            IPagingStrategyFactory pagingStrategyFactory)
        {
            this.sharedContext = sharedContext;
            this.logger = logger;
            this.pagingStrategyFactory = pagingStrategyFactory;
            this.codeSystemCodeCachingManager = codeSystemCodeCachingManager;
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

        public async Task<ICsvQueryResult> GetCodeSystemCodesBatchAsync(
            IEnumerable<string> codes,
            IEnumerable<Guid> codeSystemGuids)
        {
            var codesHash = codes.ToHashSet();

            var found = new List<ICodeSystemCode>();
            var systemGuids = codeSystemGuids as Guid[] ?? codeSystemGuids.ToArray();

            foreach (var batch in codesHash.Batch(500))
            {
                var results = await this.GetCodesByBatch(batch.ToList(), systemGuids);
                var unique = results.Where(r => !found.Exists(f => f.CodeGuid == r.CodeGuid));
                found.AddRange(unique);
            }

            return new CsvQueryResult
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
            var systemGuids = codeSystemGuids as Guid[] ?? codeSystemGuids.ToArray();
            if (systemGuids.Any())
            {
                dtos = dtos.Where(dto => systemGuids.Contains(dto.CodeSystemGUID));
            }

            if (!filterText.IsNullOrWhiteSpace())
            {
                dtos = dtos.Where(dto => dto.CodeDSC.Contains(filterText) || dto.CodeCD.StartsWith(filterText));
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

            var factory = new CodeSystemCodeFactory();
            var results = await dtos.ToListAsync();

            return results.Select(factory.Build).ToList();
        }

        private IQueryable<CodeSystemCodeDto> GetBaseQuery(bool includeRetired)
        {
            return includeRetired
                       ? this.sharedContext.CodeSystemCodes.Include(codeDto => codeDto.CodeSystem)
                       : this.sharedContext.CodeSystemCodes.Include(codeDto => codeDto.CodeSystem)
                           .Where(dto => dto.RetiredFLG == "N");
        }

        private ICodeSystemCode QueryCodeSystemCode(Guid codeGuid)
        {
            var factory = new CodeSystemCodeFactory();
            var dto = this.GetBaseQuery(true).SingleOrDefault(d => d.CodeGUID == codeGuid);
            return dto != null ? factory.Build(dto) : null;
        }

        private IReadOnlyCollection<ICodeSystemCode> QueryCodeSystemCodeList(bool includeRetired, Guid[] codeGuids)
        {
            if (!codeGuids.Any())
            {
                return new List<ICodeSystemCode>();
            }

            var factory = new CodeSystemCodeFactory();
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

            var count = await source.CountAsync();
            var items = await source.OrderBy(dto => dto.CodeDSC)
                            .Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage)
                            .Take(pagerSettings.ItemsPerPage)
                            .ToListAsync();

            var factory = new CodeSystemCodeFactory();

            return pagingStrategy.CreatePagedCollection(
                items.Select(i => this.codeSystemCodeCachingManager.GetOrSet(i.CodeGUID, () => factory.Build(i))
                ).Values(),
                count,
                pagerSettings);
        }
    }
}
