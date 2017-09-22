namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Models;
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
            var valueSetCodeRepository = new SqlValueSetCodeRepository(
                this.SharedContext,
                this.Logger,
                new ValueSetCachingManager<IValueSetCode>(this.Cache));

            var valueSetCodeCountRepository = new SqlValueSetCodeCountRepository(
                this.SharedContext,
                this.Logger,
                new ValueSetCachingManager<IValueSetCodeCount>(this.Cache));

            var valueSetBackingItemRepository = new SqlValueSetBackingItemRepository(
                this.SharedContext,
                this.Logger,
                new ValueSetCachingManager<IValueSetBackingItem>(this.Cache),
                new PagingStrategyFactory());

            this.ValueSetService = new ValueSetService(this.Logger, valueSetBackingItemRepository, valueSetCodeRepository);

            this.ValueSetSummaryService = new ValueSetSummaryService(this.Logger, valueSetBackingItemRepository, valueSetCodeCountRepository);
        }
    }
}