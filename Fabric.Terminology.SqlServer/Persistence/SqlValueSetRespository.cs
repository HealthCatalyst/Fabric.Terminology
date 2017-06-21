using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
        private readonly IValueSetCodeRepository _valueSetCodeRepository;

        public SqlValueSetRespository(SharedContext sharedContext, IMemoryCacheProvider cache, IValueSetCodeRepository valueSetCodeRepository) 
            : base(sharedContext, cache)
        {
            _valueSetCodeRepository = valueSetCodeRepository ?? throw new NullReferenceException(nameof(valueSetCodeRepository));
        }
        
        protected override Expression<Func<ValueSetDescriptionDto, string>> SortExpression => sortBy => sortBy.ValueSetNM;
        protected override SortDirection Direction { get; } = SortDirection.Ascending;
        protected override DbSet<ValueSetDescriptionDto> DbSet => SharedContext.ValueSetDescriptions;

        public IValueSet GetValueSet(string valueSetId)
        {
            throw new System.NotImplementedException();
        }

        Task<IReadOnlyCollection<IValueSet>> IValueSetRepository.GetValueSets(params string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetValueSets(IPagerSettings pagerSettings)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetValueSets(string nameFilterText, IPagerSettings pagerSettings)
        {
            throw new NotImplementedException();
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