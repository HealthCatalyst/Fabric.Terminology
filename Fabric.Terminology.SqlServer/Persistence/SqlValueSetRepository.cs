namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Factories;

    using Serilog;

    internal class SqlValueSetRepository : IValueSetRepository
    {
        private readonly SharedContext sharedContext;

        private readonly Lazy<ClientTermContext> clientTermContext;

        private readonly IMemoryCacheProvider cache;

        private readonly ILogger logger;

        private readonly IValueSetCodeRepository valueSetCodeRepository;

        private readonly IPagingStrategyFactory pagingStrategyFactory;

        public SqlValueSetRepository(
            SharedContext sharedContext,
            Lazy<ClientTermContext> clientTermContext,
            IMemoryCacheProvider cache,
            ILogger logger,
            IValueSetCodeRepository valueSetCodeRepository,
            IPagingStrategyFactory pagingStrategyFactory)
        {
            this.sharedContext = sharedContext;
            this.clientTermContext = clientTermContext;
            this.cache = cache;
            this.logger = logger;
            this.valueSetCodeRepository = valueSetCodeRepository;
            this.pagingStrategyFactory = pagingStrategyFactory;
        }

        public bool NameExists(string name)
        {
            return this.sharedContext.ValueSetDescriptions.Any(dto => dto.ValueSetNM == name);
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid)
        {
            throw new NotImplementedException();
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<IValueSet> GetValueSets(IEnumerable<Guid> valueSetUniqueGuids, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(string filterText, IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Attempt<IValueSet> Add(IValueSet valueSet)
        {
            throw new NotImplementedException();
        }

        public void Delete(IValueSet valueSet)
        {
            throw new NotImplementedException();
        }

        private Maybe<IValueSetBackingItem> QueryValueSetBackingItem(Guid valueSetGuid)
        {
            var factory = new ValueSetBackingItemFactory();

            try
            {
                return Maybe.From(this.sharedContext.ValueSetDescriptions.FirstOrDefault(dto => dto.ValueSetGUID == valueSetGuid))
                        .Select(factory.Build);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Query failed for ValueSetDescription by ValueSetGUID");
                throw;
            }
        }
    }
}