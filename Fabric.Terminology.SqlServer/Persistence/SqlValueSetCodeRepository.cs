using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fabric.Terminology.Domain;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Models.Dto;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Fabric.Terminology.SqlServer.Persistence.Mapping;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fabric.Terminology.SqlServer.Persistence
{
    using System.Text;

    using Microsoft.EntityFrameworkCore.Storage;

    internal class SqlValueSetCodeRepository : SqlPageableRepositoryBase<ValueSetCodeDto, IValueSetCode>, IValueSetCodeRepository
    {
        public SqlValueSetCodeRepository(SharedContext sharedContext, ILogger logger) 
            : base(sharedContext, logger)
        {
        }

        protected override Expression<Func<ValueSetCodeDto, string>> SortExpression => sortBy => sortBy.CodeDSC;

        protected override SortDirection Direction { get; } = SortDirection.Ascending;

        protected override DbSet<ValueSetCodeDto> DbSet => this.SharedContext.ValueSetCodes;

        public int CountValueSetCodes(string valueSetId)
        {
            return this.DbSet.Count(dto => dto.ValueSetID == valueSetId);
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(string valueSetId)
        {
            var dtos = this.DbSet
                .Where(dto => dto.ValueSetID.Equals(valueSetId))
                .OrderBy(this.SortExpression);

            var mapper = new ValueSetCodeMapper();

            return dtos.Select(dto => mapper.Map(dto)).ToList().AsReadOnly();
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodes(string valueSetId, IPagerSettings settings)
        {
            var dtos = this.DbSet.Where(dto => dto.ValueSetID == valueSetId);
            return this.CreatePagedCollectionAsync(dtos, settings, new ValueSetCodeMapper());
        }

        /// <remarks>
        /// Entity Framework does not support PARTITION BY and "will most likely generate the query using CROSS APPLY"
        /// Attempt to use straight also resulted in several warnings indicating that certain portions of the query could
        /// not be translated and the expression would be evaluated in the CLR after the execution (so no performance gain).
        /// </remarks>
        /// <seealso cref="https://stackoverflow.com/questions/43906840/row-number-over-partition-by-order-by-in-entity-framework"/>
        public Task<ILookup<string, IValueSetCode>> LookupValueSetCodes(IEnumerable<string> valueSetIds, int count = 5) // TODO confirm with Cody , params string[] codeSystemCodes
        {
            var setIds = valueSetIds as string[] ?? valueSetIds.ToArray();
            if (!setIds.Any())
            {
                return Task.FromResult(Enumerable.Empty<IValueSetCode>().ToLookup(vs => vs.ValueSetId, vs => vs));
            }

            var mapper = new ValueSetCodeMapper();

            var innerSql = new StringBuilder("SELECT vsc.BindingID, ")
                .Append("vsc.BindingNM, ")
                .Append("vsc.CodeCD, ")
                .Append("vsc.CodeDSC, ")
                .Append("vsc.CodeSystemCD, ")
                .Append("vsc.CodeSystemNM, ")
                .Append("vsc.CodeSystemVersionTXT, ")
                .Append("vsc.LastLoadDTS, ")
                .Append("vsc.RevisionDTS, ")
                .Append("vsc.SourceDSC, ")
                .Append("vsc.ValueSetID, ")
                .Append("vsc.ValueSetNM, ")
                .Append("vsc.ValueSetOID, ")
                .Append("vsc.VersionDSC, ")
                .Append("ROW_NUMBER() OVER (PARTITION BY vsc.ValueSetID ORDER BY vsc.ValueSetID) AS rownum ")
                .Append("FROM [Terminology].[ValueSetCode] vsc ")
                .Append("WHERE vsc.ValueSetID ")
                .AppendInClause(setIds);

            //if (codeSystemCodes.Any())
            //{
            //    innerSql.Append(" AND vsc.CodeSystemCD ").AppendInClause(codeSystemCodes);
            //}

            var sql = new StringBuilder("SELECT vscr.BindingID, ")
                .Append("vscr.BindingNM, ")
                .Append("vscr.CodeCD, ")
                .Append("vscr.CodeDSC, ")
                .Append("vscr.CodeSystemCD, ")
                .Append("vscr.CodeSystemNM,")
                .Append("vscr.CodeSystemVersionTXT, ")
                .Append("vscr.LastLoadDTS, ")
                .Append("vscr.RevisionDTS, ")
                .Append("vscr.SourceDSC, ")
                .Append("vscr.ValueSetID, ")
                .Append("vscr.ValueSetNM, ")
                .Append("vscr.ValueSetOID, ")
                .Append("vscr.VersionDSC, ")
                .Append("vscr.rownum ")
                .AppendFormat("FROM ({0}) vscr ", innerSql.ToString())
                .AppendFormat("WHERE vscr.rownum <= {0} ", count);

            this.Logger.Debug(sql.ToString());

            return Task.Run(() => this.DbSet.FromSql(sql.ToString()).ToLookup(vsc => vsc.ValueSetID, vsc => mapper.Map(vsc)));
        }


        private async Task<PagedCollection<IValueSetCode>> CreatePagedCollectionAsync(IQueryable<ValueSetCodeDto> source, IPagerSettings pagerSettings, IModelMapper<ValueSetCodeDto, IValueSetCode> mapper)
        {
            this.EnsurePagerSettings(pagerSettings);

            var count = await source.CountAsync();
            var items = await source.OrderBy(this.SortExpression).Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage).Take(pagerSettings.ItemsPerPage).ToListAsync();

            return this.CreatePagedCollection(items, count, pagerSettings, mapper);
        }
    }
}