using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Fabric.Terminology.Domain;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Fabric.Terminology.SqlServer.Persistence
{
    internal class SqlValueSetRespository : SqlPageableRepositoryBase<ValueSetDescriptionDto>, IValueSetRepository
    {
        public SqlValueSetRespository(SharedContext sharedContext, IMemoryCacheProvider cache) 
            : base(sharedContext, cache)
        {
        }
        
        protected override Expression<Func<ValueSetDescriptionDto, string>> SortExpression => sortBy => sortBy.ValueSetNM;
        protected override SortDirection Direction { get; } = SortDirection.Ascending;
        protected override DbSet<ValueSetDescriptionDto> DbSet => SharedContext.ValueSetDescriptions;

        public IValueSet GetValueSet(string valueSetId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IValueSet> GetValueSets(params string[] ids)
        {
            throw new System.NotImplementedException();
        }
    }
}