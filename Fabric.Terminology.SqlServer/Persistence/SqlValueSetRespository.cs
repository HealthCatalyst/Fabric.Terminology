using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fabric.Terminology.Domain;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Models.Dto;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Fabric.Terminology.SqlServer.Persistence.Mapping;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fabric.Terminology.SqlServer.Persistence
{
    internal class SqlValueSetRespository : SqlPageableRepositoryBase<ValueSetDescriptionDto, IValueSet>, IValueSetRepository
    {
        private readonly IValueSetCodeRepository valueSetCodeRepository;

        public SqlValueSetRespository(SharedContext sharedContext, IMemoryCacheProvider cache, ILogger logger, IValueSetCodeRepository valsetCodeRepository) 
            : base(sharedContext, logger)
        {
            this.valueSetCodeRepository = valsetCodeRepository;
            this.Cache = cache;
        }

        protected virtual IMemoryCacheProvider Cache { get; }

        protected override Expression<Func<ValueSetDescriptionDto, string>> SortExpression => sortBy => sortBy.ValueSetNM;
        protected override SortDirection Direction { get; } = SortDirection.Ascending;
        protected override DbSet<ValueSetDescriptionDto> DbSet => this.SharedContext.ValueSetDescriptions;

        [CanBeNull]
        public IValueSet GetValueSet(string valueSetId)
        {
            var dto = this.DbSet.FirstOrDefault(q => q.ValueSetID == valueSetId);
            if (dto == null) return null;

            var mapper = new ValueSetMapper(this.Cache, this.valueSetCodeRepository);

            var valueSet = mapper.Map(dto);
            ((ValueSet) valueSet).ValueSetCodes = this.valueSetCodeRepository.GetValueSetCodes(valueSetId);
            return valueSet;
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings pagerSettings)
        {
            return this.FindValueSetsAsync(string.Empty, pagerSettings);
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings)
        {
            var dtos = !nameFilterText.IsNullOrWhiteSpace()
                ? this.DbSet.Where(dto => dto.ValueSetNM.Contains(nameFilterText))
                : this.DbSet;

            var ids = dtos.OrderBy(this.SortExpression).Select(dto => dto.ValueSetID).Skip((pagerSettings.CurrentPage - 1) * pagerSettings.ItemsPerPage).Take(pagerSettings.ItemsPerPage);

            return this.CreatePagedCollectionAsync(dtos, pagerSettings, new ValueSetMapper(this.Cache, this.valueSetCodeRepository));
        }
    }
}