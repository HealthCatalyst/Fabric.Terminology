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

        public IValueSetCode GetCode(string code, string codeSytemCode, string valueSetId)
        {
            var dto = DbSet
                        .Where(q => q.CodeCD.Equals(code) && q.CodeSystemCD.Equals(codeSytemCode) && q.ValueSetID.Equals(valueSetId))
                        .OrderByDescending(q => q.RevisionDTS)
                        .SingleOrDefault();

            return dto != null ? MapToResult(dto) : null;
        }

        public IReadOnlyCollection<IValueSetCode> GetCodes(string code)
        {
            var dtos = DbSet.Where(dto => dto.CodeCD.Equals(code)).AsNoTracking().ToList();
            return dtos.Select(MapToResult).ToList().AsReadOnly();
        }

        public Task<PagedCollection<IValueSetCode>> GetCodesAsync(IPagerSettings settings)
        {
            return FindCodesAsync(string.Empty, settings);
        }

        public Task<PagedCollection<IValueSetCode>> GetCodesAsync(string codeSystemCode, IPagerSettings settings)
        {
            return GetCodesAsync(new[] { codeSystemCode }, settings);
        }

        public Task<PagedCollection<IValueSetCode>> GetCodesAsync(string[] codeSystemCodes, IPagerSettings settings)
        {
            return FindCodesAsync(string.Empty, codeSystemCodes, settings);
        }

        public Task<PagedCollection<IValueSetCode>> FindCodesAsync(string codeNameFilterText, IPagerSettings settings)
        {
            EnsurePagerSettings(settings);

            var dtos = !codeNameFilterText.IsNullOrWhiteSpace() ? 
                        DbSet.Where(dto => dto.CodeDSC.Equals(codeNameFilterText)) : 
                        DbSet;

            return CreatePagedCollectionAsync(dtos, settings);
        }

        public Task<PagedCollection<IValueSetCode>> FindCodesAsync(string codeNameFilterText, string codeSystemCode, IPagerSettings settings)
        {
            return FindCodesAsync(codeNameFilterText, new[] {codeSystemCode}, settings);
        }

        public Task<PagedCollection<IValueSetCode>> FindCodesAsync(string codeNameFilterText, string[] codeSystemCodes, IPagerSettings settings)
        {

            EnsurePagerSettings(settings);

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

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(string valueSetId)
        {
            // Memory cache check is here
            var cacheKey = CacheKeys.ValueSetCodesKey(valueSetId);

            return (IReadOnlyCollection<IValueSetCode>)
                Cache.GetItem(cacheKey, () =>
                    {
                        var dtos = DbSet
                            .Where(dto => dto.ValueSetID.Equals(valueSetId))
                            .OrderBy(SortExpression)
                            .AsNoTracking();

                        return dtos.Select(dto => MapToResult(dto)).ToList().AsReadOnly();
                    },
                    TimeSpan.FromMinutes(5),
                    true);

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