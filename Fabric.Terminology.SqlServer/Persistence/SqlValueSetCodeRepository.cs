namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlValueSetCodeRepository : IValueSetCodeRepository
    {
        private readonly SharedContext sharedContext;

        private readonly ILogger logger;

        private readonly IValueSetCachingManager cacheManager;

        public SqlValueSetCodeRepository(
            SharedContext sharedContext,
            ILogger logger,
            IValueSetCachingManager cacheManager)
        {
            this.sharedContext = sharedContext;
            this.logger = logger;
            this.cacheManager = cacheManager;
        }

        public int CountValueSetCodes(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids)
        {
            return this.GetValueSetCodes(valueSetGuid, codeSystemGuids).Count();
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid)
        {
            return this.cacheManager
                    .GetOrQuery(valueSetGuid, CacheKeys.ValueSetCodesKey, this.QueryValueSetCodes)
                    .OrderBy(code => code.Name)
                    .ToList();
        }

        public IReadOnlyCollection<IValueSetCodeCount> GetValueSetCodeCounts(Guid valueSetGuid)
        {
            return this.cacheManager.GetOrQuery(valueSetGuid, CacheKeys.ValueSetCodesKey, this.QueryValueSetCodeCounts);
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid, IEnumerable<Guid> codeSystsemGuids)
        {
            var systsemGuids = codeSystsemGuids as Guid[] ?? codeSystsemGuids.ToArray();
            return systsemGuids.Any()
                       ? this.GetValueSetCodes(valueSetGuid)
                           .Where(code => systsemGuids.Contains(code.CodeSystemGuid))
                           .ToList()
                       : this.GetValueSetCodes(valueSetGuid).ToList();
        }

        public Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCodeCount>>> BuildValueSetCountsDictionary(IEnumerable<Guid> valueSetGuids)
        {
            return this.cacheManager.GetCachedValueDictionary(
                valueSetGuids,
                CacheKeys.ValueSetCodeCountsKey,
                this.QueryValueSetCodeCountLookup);
        }

        public Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCode>>> BuildValueSetCodesDictionary(IEnumerable<Guid> valueSetGuids)
        {
            return this.cacheManager.GetCachedValueDictionary(
                valueSetGuids,
                CacheKeys.ValueSetCodesKey,
                this.QueryValueSetCodeLookup);
        }

        private IReadOnlyCollection<IValueSetCode> QueryValueSetCodes(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeFactory();

            try
            {
                return this.sharedContext.ValueSetCodes.Where(dto => dto.ValueSetGUID == valueSetGuid)
                    .Select(dto => factory.Build(dto))
                    .ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query ValueSetCodes by ValueSetGUID");
                throw;
            }
        }

        private IReadOnlyCollection<IValueSetCodeCount> QueryValueSetCodeCounts(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeCountFactory();

            try
            {
                return this.sharedContext.ValueSetCounts.Where(dto => dto.ValueSetGUID == valueSetGuid)
                    .Select(dto => factory.Build(dto))
                    .ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query ValueSetCodeCounts for ValueSetGUID");
                throw;
            }
        }

        private ILookup<Guid, IValueSetCode> QueryValueSetCodeLookup(IEnumerable<Guid> valueSetGuids)
        {
            var factory = new ValueSetCodeFactory();

            try
            {
                return this.sharedContext.ValueSetCodes.Where(dto => valueSetGuids.Contains(dto.ValueSetGUID))
                    .AsNoTracking()
                    .ToLookup(vsc => vsc.ValueSetGUID, vsc => factory.Build(vsc));
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query for ValueSetCode lookup for collection of ValueSetGUIDs");
                throw;
            }
        }

        private ILookup<Guid, IValueSetCodeCount> QueryValueSetCodeCountLookup(IEnumerable<Guid> valueSetGuids)
        {
            var factory = new ValueSetCodeCountFactory();

            try
            {
                return this.sharedContext.ValueSetCounts
                    .Where(dto => valueSetGuids.Contains(dto.ValueSetGUID))
                    .AsNoTracking()
                    .ToLookup(vscc => vscc.ValueSetGUID, vscc => factory.Build(vscc));
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to query for ValueSetCodeCount lookup for collection of ValueSetGUIDs");
                throw;
            }
        }
    }
}