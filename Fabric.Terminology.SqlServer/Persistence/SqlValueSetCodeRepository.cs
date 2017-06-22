using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fabric.Terminology.Domain;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Models.Dto;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fabric.Terminology.SqlServer.Persistence
{
    internal class SqlValueSetCodeRepository : SqlPageableRepositoryBase<ValueSetCodeDto, IValueSetCode>, IValueSetCodeRepository
    {
        public SqlValueSetCodeRepository(SharedContext sharedContext, ILogger logger, IMemoryCacheProvider cache) 
            : base(sharedContext, logger, cache)
        {
        }

        protected override Expression<Func<ValueSetCodeDto, string>> SortExpression => sortBy => sortBy.CodeDSC;

        protected override SortDirection Direction { get; } = SortDirection.Ascending;

        protected override DbSet<ValueSetCodeDto> DbSet => SharedContext.ValueSetCodes;

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(string valueSetId)
        {
            var dtos = DbSet
                .Where(dto => dto.ValueSetID.Equals(valueSetId))
                .OrderBy(SortExpression)
                .AsNoTracking();

            return dtos.Select(dto => MapToResult(dto)).ToList().AsReadOnly();
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodes(string valueSetId, IPagerSettings settings)
        {
            var dtos = DbSet.Where(dto => dto.ValueSetID.Equals(valueSetId));
            return CreatePagedCollectionAsync(dtos, settings);
        }

        protected override IValueSetCode MapToResult(ValueSetCodeDto dto)
        {
            var codeSystem = new ValueSetCodeSystem
            {
                Code = dto.CodeSystemCD,
                Name = dto.CodeSystemNM,
                Version = dto.CodeSystemVersionTXT
            };

            return new ValueSetCode
            {
                ValueSetId = dto.ValueSetID,
                Code = dto.CodeCD,
                CodeSystem = codeSystem,
                Name = dto.CodeDSC,
                RevisionDate = dto.RevisionDTS,
                VersionDescription = dto.VersionDSC
            };
        }
    }
}