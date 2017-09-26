namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.TestsBase.Fixtures;

    public class ValueSetBackingItemRepositoryFixture : RepositoryFixtureBase
    {
        public ValueSetBackingItemRepositoryFixture()
        {
            this.Initialize();
        }

        internal IValueSetBackingItemRepository ValueSetBackingItemRepository { get; private set; }

        private void Initialize()
        {
            var cachingManagerFactory = new CachingManagerFactory(this.Cache);

            this.ValueSetBackingItemRepository = new SqlValueSetBackingItemRepository(
                this.SharedContext,
                this.Logger,
                cachingManagerFactory,
                new PagingStrategyFactory());
        }
    }
}
