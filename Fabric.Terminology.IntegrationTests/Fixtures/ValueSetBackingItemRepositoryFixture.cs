namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Persistence;
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
            this.ValueSetBackingItemRepository = new SqlValueSetBackingItemRepository(this.SharedContext, this.Cache, this.Logger, new PagingStrategyFactory());
        }
    }
}
