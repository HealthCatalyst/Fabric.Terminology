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
    internal class SqlValueSetCodeRepository : SqlPageableRepositoryBase<ValueSetCodeDto, IValueSetCode>, IValueSetCodeRepository
    {
        public SqlValueSetCodeRepository(SharedContext sharedContext, ILogger logger) 
            : base(sharedContext, logger)
        {
        }

        protected override Expression<Func<ValueSetCodeDto, string>> SortExpression => sortBy => sortBy.CodeDSC;

        protected override SortDirection Direction { get; } = SortDirection.Ascending;

        protected override DbSet<ValueSetCodeDto> DbSet => this.SharedContext.ValueSetCodes;

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(string valueSetId)
        {
            var dtos = this.DbSet
                .Where(dto => dto.ValueSetID.Equals(valueSetId))
                .OrderBy(this.SortExpression)
                .AsNoTracking();

            var mapper = new ValueSetCodeMapper();

            return dtos.Select(mapper.Map).ToList().AsReadOnly();
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodes(string valueSetId, IPagerSettings settings)
        {
            var dtos = this.DbSet.Where(dto => dto.ValueSetID == valueSetId);
            return this.CreatePagedCollectionAsync(dtos, settings, new ValueSetCodeMapper());
        }

        public ILookup<string, IValueSetCode> LookupValueSetCodes(IQueryable<string> valueSetIds, int count = 5)
        {
            var ids = valueSetIds.ToList();

            var mapper = new ValueSetCodeMapper();

            var dtos = this.DbSet
                .Where(dto => ids.Contains(dto.ValueSetID))
                .GroupBy(dto => dto.ValueSetID)
                .Select(dto => dto.OrderBy(x => x.CodeDSC).Take(count))
                .SelectMany(dto => dto)
                .ToLookup(dto => dto.ValueSetID, dto => mapper.Map(dto));

            return dtos;
        }
    }
}