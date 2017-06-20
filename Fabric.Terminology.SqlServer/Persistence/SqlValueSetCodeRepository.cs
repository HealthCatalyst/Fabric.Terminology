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
using Fabric.Terminology.SqlServer.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Fabric.Terminology.SqlServer.Persistence
{
    internal class SqlValueSetCodeRepository : SqlPageableRepositoryBase<ValueSetCodeDto, IValueSetCode>, IValueSetCodeRepository
    {
        public SqlValueSetCodeRepository(SharedContext sharedContext, IMemoryCacheProvider cache) 
            : base(sharedContext, cache)
        {
        }

        protected override Expression<Func<ValueSetCodeDto, string>> SortExpression => sortBy => sortBy.CodeDSC;

        protected override SortDirection Direction { get; } = SortDirection.Ascending;

        protected override DbSet<ValueSetCodeDto> DbSet => SharedContext.ValueSetCodes;

        public IReadOnlyCollection<IValueSetCode> GetCodesByValueSet(string valueSetId)
        {
            // Memory cache check is here

            var dtos = DbSet
                .Where(dto => dto.ValueSetID.Equals(valueSetId))
                .OrderBy(SortExpression)
                .AsNoTracking();

            return dtos.Select(dto => MapToResult(dto)).ToList().AsReadOnly();
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string codeSystemCode, IPagerSettings settings)
        {
            return GetValueSetCodesAsync(new[] { codeSystemCode }, settings);
        }

        public Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string[] codeSytemCodes, IPagerSettings settings)
        {

            if (settings.CurrentPage <= 0) settings.CurrentPage = 1;
            if (settings.ItemsPerPage < 0) settings.ItemsPerPage = SharedContext.DefaultItemsPerPage;

            if (codeSytemCodes == null) throw new ArgumentNullException(nameof(codeSytemCodes));
            if (!codeSytemCodes.Any()) throw new InvalidOperationException("A code system must be specified.");

            var dtos = codeSytemCodes.Length > 1
                ? DbSet
                    .Where(dto => codeSytemCodes.Contains(dto.CodeSystemCD))
                : DbSet
                    .Where(dto => dto.CodeSystemCD.Equals(codeSytemCodes.First()));

            return CreatePagedCollectionAsync(dtos, settings);
        }

        internal Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string codeSystemCode, int currentPage, int itemsPerPage)
        {
            return GetValueSetCodesAsync(new[] { codeSystemCode }, new PagerSettings { CurrentPage = currentPage, ItemsPerPage = itemsPerPage });
        }

        internal Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string[] codeSytemCodes, int currentPage, int itemsPerPage)
        {
            return GetValueSetCodesAsync(codeSytemCodes, new PagerSettings { CurrentPage = currentPage, ItemsPerPage = itemsPerPage });
        }

        protected override IValueSetCode MapToResult(ValueSetCodeDto dto)
        {
            return ModelFactory.BuildModel(dto);
        }
    }
}