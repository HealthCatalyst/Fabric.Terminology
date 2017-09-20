namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    using Serilog;

    internal class SqlValueSetCodeRepository : IValueSetCodeRepository
    {
        private readonly SharedContext sharedContext;

        private readonly Lazy<ClientTermContext> clientTermContext;

        private readonly IPagingStrategyFactory pagingStrategyFactory;

        private readonly ILogger logger;

        public SqlValueSetCodeRepository(
            SharedContext sharedContext,
            Lazy<ClientTermContext> clientTermContext,
            ILogger logger,
            IPagingStrategyFactory pagingStrategyFactory)
        {
            this.sharedContext = sharedContext;
            this.clientTermContext = clientTermContext;
            this.logger = logger;
            this.pagingStrategyFactory = pagingStrategyFactory;
        }

        private Expression<Func<ValueSetCodeDto, string>> SortExpression => sortBy => sortBy.CodeDSC;


        public int CountValueSetCodes(Guid valueSetGuid, IEnumerable<string> codeSystemCodes)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid, IEnumerable<Guid> codeSystsemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<ILookup<Guid, IValueSetCodeCount>> LookupValueSetCounts(IEnumerable<Guid> valueSetGuids)
        {
            throw new NotImplementedException();
        }

        public Task<ILookup<string, IValueSetCode>> LookupValueSetCodes(IEnumerable<Guid> valueSetGuids)
        {
            throw new NotImplementedException();
        }
    }
}