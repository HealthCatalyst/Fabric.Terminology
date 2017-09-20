namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    using Serilog;

    internal class SqlValueSetRepository : IValueSetRepository
    {
        private readonly SharedContext sharedContext;

        private readonly Lazy<ClientTermContext> clientTermContext;

        private readonly IMemoryCacheProvider cache;

        private readonly ILogger logger;

        private readonly IPagingStrategyFactory pagingStrategyFactory;

        public SqlValueSetRepository(
            SharedContext sharedContext,
            Lazy<ClientTermContext> clientTermContext,
            IMemoryCacheProvider cache,
            ILogger logger,
            IPagingStrategyFactory pagingStrategyFactory)
        {
            this.sharedContext = sharedContext;
            this.clientTermContext = clientTermContext;
            this.cache = cache;
            this.logger = logger;
            this.pagingStrategyFactory = pagingStrategyFactory;
        }

        private Expression<Func<ValueSetDescriptionDto, string>> SortExpression => sortBy => sortBy.ValueSetNM;

        public bool NameExists(string name)
        {
            return this.sharedContext.ValueSetDescriptions.Any(dto => dto.ValueSetNM == name);
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<IValueSet> GetValueSets(IEnumerable<Guid> valueSetUniqueGuids, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(IEnumerable<Guid> valueSetGuids, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(string filterText, IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> FindValueSetsSummariesAsync(string filterText, IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids)
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
    }
}