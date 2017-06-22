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
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fabric.Terminology.SqlServer.Persistence
{
    internal class SqlValueSetRespository : SqlPageableRepositoryBase<ValueSetDescriptionDto, IValueSet>, IValueSetRepository
    {
        private readonly IValueSetCodeRepository _valueSetCodeRepository;

        public SqlValueSetRespository(SharedContext sharedContext, ILogger logger, IMemoryCacheProvider cache, IValueSetCodeRepository valueSetCodeRepository) 
            : base(sharedContext, logger, cache)
        {
            _valueSetCodeRepository = valueSetCodeRepository ?? throw new NullReferenceException(nameof(valueSetCodeRepository));
        }
        
        protected override Expression<Func<ValueSetDescriptionDto, string>> SortExpression => sortBy => sortBy.ValueSetNM;
        protected override SortDirection Direction { get; } = SortDirection.Ascending;
        protected override DbSet<ValueSetDescriptionDto> DbSet => SharedContext.ValueSetDescriptions;

        public IValueSet GetValueSet(string valueSetId)
        {
            var dto = DbSet.FirstOrDefault(q => q.ValueSetID.Equals(valueSetId));
            if (dto == null) return null;

            var valueSet = MapToResult(dto);
            ((ValueSet) valueSet).ValueSetCodes = _valueSetCodeRepository.GetValueSetCodes(valueSetId);
            return valueSet;
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings pagerSettings)
        {
            return FindValueSetsAsync(string.Empty, pagerSettings);
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings)
        {
            EnsurePagerSettings(pagerSettings);

            var dtos = !nameFilterText.IsNullOrWhiteSpace()
                ? DbSet.Where(dto => dto.ValueSetNM.Contains(nameFilterText))
                : DbSet;

           return  CreatePagedCollectionAsync(dtos, pagerSettings);
        }

        protected override IValueSet MapToResult(ValueSetDescriptionDto dto)
        {

            var codes = _valueSetCodeRepository.GetValueSetCodes(dto.ValueSetID);

            var valueSet = new ValueSet
            {
                ValueSetId = dto.ValueSetID,
                AuthoringSourceDescription = dto.AuthoringSourceDSC,
                Name = dto.ValueSetNM,
                IsCustom = false,
                PurposeDescription = dto.PurposeDSC,
                SourceDescription = dto.SourceDSC,
                VersionDescription = dto.VersionDSC,
                ValueSetCodes = codes,
                ValueSetCodesCount = codes.Count
            };

            //FillValueSetCodesAsync(valueSet).Wait();
            //valueSet.ValueSetCodesCount = valueSet.ValueSetCodes.Count;
            return valueSet;
        }

        private Task FillValueSetCodesAsync(ValueSet valueSet)
        {
            return Task.Run(() => valueSet.ValueSetCodes = _valueSetCodeRepository.GetValueSetCodes(valueSet.ValueSetId));
        }
    }
}