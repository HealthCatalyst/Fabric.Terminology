namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Persistence.Mapping;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlValueSetCodeRepository : IValueSetCodeRepository
    {
        private readonly IPagingStrategy<ValueSetCodeDto, IValueSetCode> pagingStrategy;

        public SqlValueSetCodeRepository(
            SharedContext sharedContext,
            ILogger logger,
            IPagingStrategy<ValueSetCodeDto, IValueSetCode> pagingStrategy)
        {
            this.SharedContext = sharedContext;
            this.Logger = logger;
            this.pagingStrategy = pagingStrategy;
        }

        protected SharedContext SharedContext { get; }

        protected ILogger Logger { get; }

        protected Expression<Func<ValueSetCodeDto, string>> SortExpression => sortBy => sortBy.CodeDSC;

        protected DbSet<ValueSetCodeDto> DbSet => this.SharedContext.ValueSetCodes;

        public int CountValueSetCodes(string valueSetId, IEnumerable<string> codeSystemCodes)
        {
            var systemCodes = codeSystemCodes as string[] ?? codeSystemCodes.ToArray();
            return systemCodes.Any()
                       ? this.DbSet.Count(
                           dto => dto.ValueSetID == valueSetId && systemCodes.Contains(dto.CodeSystemCD))
                       : this.DbSet.Count(dto => dto.ValueSetID == valueSetId);
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(
            string valueSetId,
            IEnumerable<string> codeSystemCodes)
        {
            var dtos = this.DbSet.Where(dto => dto.ValueSetID == valueSetId);

            var systemCodes = codeSystemCodes as string[] ?? codeSystemCodes.ToArray();
            if (systemCodes.Any())
            {
                dtos = dtos.Where(dto => systemCodes.Contains(dto.CodeSystemCD));
            }

            dtos = dtos.OrderBy(this.SortExpression);

            var mapper = new ValueSetCodeMapper();

            return dtos.Select(dto => mapper.Map(dto)).ToList().AsReadOnly();
        }

        /// <remarks>
        /// Entity Framework does not support PARTITION BY and "will most likely generate the query using CROSS APPLY"
        /// Attempt to use straight also resulted in several warnings indicating that certain portions of the query could
        /// not be translated and the expression would be evaluated in the CLR after the execution (so no performance gain).
        /// </remarks>
        /// <seealso cref="https://stackoverflow.com/questions/43906840/row-number-over-partition-by-order-by-in-entity-framework"/>
        public Task<ILookup<string, IValueSetCode>> LookupValueSetCodes(
            IEnumerable<string> valueSetIds,
            IEnumerable<string> codeSystemCodes,
            int count = 5)
        {
            var setIds = valueSetIds as string[] ?? valueSetIds.ToArray();
            if (!setIds.Any())
            {
                return Task.FromResult(Enumerable.Empty<IValueSetCode>().ToLookup(vs => vs.ValueSetId, vs => vs));
            }

            var mapper = new ValueSetCodeMapper();

            var escapedSetIds = string.Join(",", setIds.Select(EscapeForSqlString).Select(v => "'" + v + "'"));

            var innerSql =
                $@"SELECT vsc.BindingID, vsc.BindingNM, vsc.CodeCD, vsc.CodeDSC, vsc.CodeSystemCD, vsc.CodeSystemNM, vsc.CodeSystemVersionTXT,
vsc.LastLoadDTS, vsc.RevisionDTS, vsc.SourceDSC, vsc.ValueSetID, vsc.ValueSetNM, vsc.ValueSetOID, vsc.VersionDSC,
ROW_NUMBER() OVER (PARTITION BY vsc.ValueSetID ORDER BY vsc.ValueSetID) AS rownum 
FROM [Terminology].[ValueSetCode] vsc WHERE vsc.ValueSetID IN ({escapedSetIds})";

            var systemCodes = codeSystemCodes as string[] ?? codeSystemCodes.ToArray();
            if (systemCodes.Any())
            {
                var escapedCodes = string.Join(
                    ",",
                    systemCodes.Select(EscapeForSqlString).Select(v => "'" + v + "'"));
                innerSql += $" AND vsc.CodeSystemCD IN ({escapedCodes})";
            }

            var sql =
                $@"SELECT vscr.BindingID, vscr.BindingNM, vscr.CodeCD, vscr.CodeDSC, vscr.CodeSystemCD, vscr.CodeSystemNM, vscr.CodeSystemVersionTXT,
vscr.LastLoadDTS, vscr.RevisionDTS, vscr.SourceDSC, vscr.ValueSetID, vscr.ValueSetNM, vscr.ValueSetOID, vscr.VersionDSC, vscr.rownum
FROM ({innerSql}) vscr
WHERE vscr.rownum <= {count}
ORDER BY vscr.CodeDSC";

            return Task.Run(() => this.DbSet.FromSql(sql).ToLookup(vsc => vsc.ValueSetID, vsc => mapper.Map(vsc)));
        }

        private async Task<PagedCollection<IValueSetCode>> CreatePagedCollectionAsync(
            IQueryable<ValueSetCodeDto> source,
            IPagerSettings pagerSettings,
            IModelMapper<ValueSetCodeDto, IValueSetCode> mapper)
        {
            this.pagingStrategy.EnsurePagerSettings(pagerSettings);

            var count = await source.CountAsync();
            var items = await source.OrderBy(this.SortExpression)
                            .Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage)
                            .Take(pagerSettings.ItemsPerPage)
                            .ToListAsync();

            return this.pagingStrategy.CreatePagedCollection(items, count, pagerSettings, mapper);
        }

        private static string EscapeForSqlString(string input)
        {
            return input.Replace("'", "''");
        }
    }
}