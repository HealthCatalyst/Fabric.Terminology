using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Fabric.Terminology.Domain;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Models.Dto;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Fabric.Terminology.SqlServer.Persistence
{
    internal class SqlValueSetRespository : SqlPageableRepositoryBase<ValueSetDescriptionDto, IValueSet>, IValueSetRepository
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

        public IReadOnlyCollection<IValueSet> GetValueSets(params string[] ids)
        {
            throw new System.NotImplementedException();
        }

        protected override IValueSet MapToResult(ValueSetDescriptionDto dto)
        {
            throw new NotImplementedException();
        }
    }
}