using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fabric.Terminology.Domain;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Fabric.Terminology.SqlServer.Persistence
{
    internal class SqlValueSetCodeRepository : SqlPageableRepositoryBase<ValueSetCodeDto>, IValueSetCodeRepository
    {
        public SqlValueSetCodeRepository(SharedContext sharedContext, IMemoryCacheProvider cache) 
            : base(sharedContext, cache)
        {
        }

        protected override Expression<Func<ValueSetCodeDto, string>> SortExpression => sortBy => sortBy.ValueSetNM;

        protected override SortDirection Direction { get; } = SortDirection.Ascending;

        protected override DbSet<ValueSetCodeDto> DbSet => SharedContext.ValueSetCodes;

        public IEnumerable<IValueSetCode> GetCodesByValueSet(string valueSetId)
        {
            // TODO check cached for this since this will be used by the other service as well.

            var dtos = DbSet
                .AsNoTracking()
                .Where(x => x.ValueSetID.Equals(valueSetId, StringComparison.OrdinalIgnoreCase))
                .OrderBy(SortExpression);

            return Enumerable.Empty<IValueSetCode>();
        }

        public PagedCollection<IValueSetCode> GetValueSetCodes(string codeSystemCode, IPagerSettings settings)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IValueSetCode> GetAll()
        {
            return Enumerable.Empty<IValueSetCode>();
        }

        public IEnumerable<IValueSetCode> GetAll(string codeSystemCode)
        {
            return Enumerable.Empty<IValueSetCode>();
        }
    }
}