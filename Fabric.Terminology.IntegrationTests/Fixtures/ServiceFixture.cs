namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence;
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

            this.ValueSetService = new SqlValueSetService(this.Logger, valueSetBackingItemRepository, valueSetCodeRepository, valueSetCodeCountRepository);

            this.ValueSetSummaryService = new SqlValueSetSummaryService(this.Logger, valueSetBackingItemRepository, valueSetCodeCountRepository);
        }
    }
}