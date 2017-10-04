namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.TestsBase.Fixtures;

    public class ServiceFixture : RepositoryFixtureBase
    {
        public ServiceFixture()
        {
            this.Initialize();
        }

        public IValueSetService ValueSetService { get; private set; }

        public IValueSetSummaryService ValueSetSummaryService { get; private set; }

        private void Initialize()
        {
            var cacheManagerFactory = new CachingManagerFactory(this.Cache);

            var valueSetCodeRepository = new SqlValueSetCodeRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory);

            var valueSetCodeCountRepository = new SqlValueSetCodeCountRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory);

            var valueSetBackingItemRepository = new SqlValueSetBackingItemRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory,
                new PagingStrategyFactory());

            var sqlClientTermValueSetRepository = new SqlClientTermValueSetRepository(
                this.ClientTermContext.AsLazy(),
                this.Logger);

            this.ValueSetService = new ValueSetService(
                this.Logger,
                valueSetBackingItemRepository,
                valueSetCodeRepository,
                valueSetCodeCountRepository,
                sqlClientTermValueSetRepository);

            this.ValueSetSummaryService = new ValueSetSummaryService(
                this.Logger,
                valueSetBackingItemRepository,
                valueSetCodeCountRepository);
        }
    }
}