namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Persistence.Querying;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.SqlServer.Persistence.Ordering;
    using Fabric.Terminology.SqlServer.Services;
    using Fabric.Terminology.TestsBase.Fixtures;

    public class ValueSetComparisonServiceFixture : RepositoryFixtureBase
    {
        public ValueSetComparisonServiceFixture()
        {
            this.Initialize();
        }

        public IValueSetComparisonService ValueSetComparisonService { get; private set; }

        private void Initialize()
        {
            var cacheManagerFactory = new CachingManagerFactory(this.Cache);
            var pagingStrategyFactory = new PagingStrategyFactory();
            var orderingStrategyFacotry = new OrderingStrategyFactory();

            var valueSetBackingItemRepository = new SqlValueSetBackingItemRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory,
                pagingStrategyFactory,
                orderingStrategyFacotry);

            var valueSetCodeRepository = new SqlValueSetCodeRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory,
                pagingStrategyFactory,
                orderingStrategyFacotry);

            var valueSetCodeCountRepository = new SqlValueSetCodeCountRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory);

            var valueSetService = new SqlValueSetService(
                this.Logger,
                valueSetBackingItemRepository,
                valueSetCodeRepository,
                valueSetCodeCountRepository);

            this.ValueSetComparisonService = new ValueSetComparisonService(valueSetService);
        }
    }
}