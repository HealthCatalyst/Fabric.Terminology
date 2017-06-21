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

        public IValueSetCode GetCode(string code)
        {
            var dto = DbSet.SingleOrDefault(q => q.CodeDSC.Equals(code));
            return dto != null ? MapToResult(dto) : null;
        }

        public IReadOnlyCollection<IValueSetCode> GetByValueSet(string valueSetId)
        {
            // Memory cache check is here

            var dtos = DbSet
                .Where(dto => dto.ValueSetID.Equals(valueSetId))
                .OrderBy(SortExpression)
                .AsNoTracking();

            return dtos.Select(dto => MapToResult(dto)).ToList().AsReadOnly();
        }

        public Task<PagedCollection<IValueSetCode>> GetByCodeSystemAsync(string codeSystemCode, IPagerSettings settings)
        {
            return GetByCodeSystemAsync(new[] { codeSystemCode }, settings);
        }

        public Task<PagedCollection<IValueSetCode>> GetByCodeSystemAsync(string codeNameFilterText, string codeSystemCode, IPagerSettings settings)
        {
            return GetByCodeSystemAsync(codeNameFilterText, new[] {codeSystemCode}, settings);
        }

        public Task<PagedCollection<IValueSetCode>> GetByCodeSystemAsync(string[] codeSystemCodes, IPagerSettings settings)
        {
            return GetByCodeSystemAsync(string.Empty, codeSystemCodes, settings);
        }

        public Task<PagedCollection<IValueSetCode>> GetByCodeSystemAsync(string codeNameFilterText, string[] codeSystemCodes, IPagerSettings settings)
        {

            if (settings.CurrentPage <= 0) settings.CurrentPage = 1;
            if (settings.ItemsPerPage < 0) settings.ItemsPerPage = SharedContext.DefaultItemsPerPage;

            if (codeSystemCodes == null) throw new ArgumentNullException(nameof(codeSystemCodes));
            if (!codeSystemCodes.Any()) throw new InvalidOperationException("A code system must be specified.");

            var dtos = codeSystemCodes.Length > 1
                ? DbSet
                    .Where(dto => codeSystemCodes.Contains(dto.CodeSystemCD))
                : DbSet
                    .Where(dto => dto.CodeSystemCD.Equals(codeSystemCodes.First()));

            if (!codeNameFilterText.IsNullOrWhiteSpace())
            {
                dtos = dtos.Where(dto => dto.CodeDSC.Contains(codeNameFilterText));
            }

            return CreatePagedCollectionAsync(dtos, settings);
        }

        internal Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string codeSystemCode, int currentPage, int itemsPerPage)
        {
            return GetByCodeSystemAsync(new[] { codeSystemCode }, new PagerSettings { CurrentPage = currentPage, ItemsPerPage = itemsPerPage });
        }

        internal Task<PagedCollection<IValueSetCode>> GetValueSetCodesAsync(string[] codeSytemCodes, int currentPage, int itemsPerPage)
        {
            return GetByCodeSystemAsync(codeSytemCodes, new PagerSettings { CurrentPage = currentPage, ItemsPerPage = itemsPerPage });
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