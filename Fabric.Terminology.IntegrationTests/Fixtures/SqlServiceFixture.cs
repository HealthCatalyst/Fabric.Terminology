namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.SqlServer.Services;
    using Fabric.Terminology.TestsBase.Fixtures;

    public class SqlServiceFixture : RepositoryFixtureBase
    {
        public SqlServiceFixture()
        {
            this.Initialize();
        }

        public IValueSetService ValueSetService { get; private set; }

        public IValueSetSummaryService ValueSetSummaryService { get; private set; }

        public IClientTermValueSetService ClientTermValueSetService { get; private set; }

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

            this.ValueSetService = new SqlValueSetService(
                this.Logger,
                valueSetBackingItemRepository,
                valueSetCodeRepository,
                valueSetCodeCountRepository);

            this.ClientTermValueSetService = new SqlClientTermValueSetService(
                this.Logger,
                valueSetBackingItemRepository,
                sqlClientTermValueSetRepository);

            this.ValueSetSummaryService = new SqlValueSetSummaryService(
                this.Logger,
                valueSetBackingItemRepository,
                valueSetCodeCountRepository);
        }
    }
}