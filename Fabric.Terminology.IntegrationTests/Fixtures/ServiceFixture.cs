namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.SqlServer.Persistence.UnitOfWork;
    using Fabric.Terminology.SqlServer.Services;
    using Fabric.Terminology.TestsBase.Fixtures;

    public class ServiceFixture : RepositoryFixtureBase
    {
        public ServiceFixture()
        {
            this.Initialize();
        }

        public IValueSetService ValueSetService { get; private set; }

        public IValueSetSummaryService ValueSetSummaryService { get; private set; }

        public IClientTermValueSetService ClientTermValueSetService { get; private set; }

        public ICodeSystemService CodeSystemService { get; private set; }

        public ICodeSystemCodeService CodeSystemCodeService { get; private set; }

        private void Initialize()
        {
            var cacheManagerFactory = new CachingManagerFactory(this.Cache);
            var pagingStrategyFactory = new PagingStrategyFactory();
            var clientTermCacheManager = new ClientTermCacheManager(cacheManagerFactory);
            var uow = new ClientTermValueUnitOfWorkManager(this.ClientTermContext.AsLazy(), this.Logger);

            var valueSetCodeRepository = new SqlValueSetCodeRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory,
                pagingStrategyFactory);

            var valueSetCodeCountRepository = new SqlValueSetCodeCountRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory);

            var valueSetBackingItemRepository = new SqlValueSetBackingItemRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory,
                pagingStrategyFactory);

            var sqlClientTermUowRepository = new SqlClientTermValueSetRepository(
                this.Logger,
                uow,
                clientTermCacheManager);

            var sqlCodeSystemRepository = new SqlCodeSystemRepository(
                this.SharedContext,
                this.Logger,
                new CodeSystemCachingManager(this.Cache));

            var sqlCodeSystemCodeRepository = new SqlCodeSystemCodeRepository(
                this.SharedContext,
                this.Logger,
                new CodeSystemCodeCachingManager(this.Cache),
                pagingStrategyFactory);

            this.ValueSetService = new SqlValueSetService(
                this.Logger,
                valueSetBackingItemRepository,
                valueSetCodeRepository,
                valueSetCodeCountRepository);

            this.ClientTermValueSetService = new SqlClientTermValueSetService(
                this.Logger,
                valueSetBackingItemRepository,
                sqlClientTermUowRepository);

            this.ValueSetSummaryService = new SqlValueSetSummaryService(
                this.Logger,
                valueSetBackingItemRepository,
                valueSetCodeCountRepository);

            this.CodeSystemService = new SqlCodeSystemService(sqlCodeSystemRepository);

            this.CodeSystemCodeService = new SqlCodeSystemCodeService(sqlCodeSystemCodeRepository);
        }
    }
}