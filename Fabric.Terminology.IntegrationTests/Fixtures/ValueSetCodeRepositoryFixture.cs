namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Persistence.Querying;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.SqlServer.Persistence.Ordering;
    using Fabric.Terminology.TestsBase.Fixtures;

    public class ValueSetCodeRepositoryFixture : RepositoryFixtureBase
    {
        public ValueSetCodeRepositoryFixture()
        {
            this.Initialize();
        }

        public IValueSetCodeRepository ValueSetCodeRepository { get; private set; }

        private void Initialize()
        {
            this.ValueSetCodeRepository = new SqlValueSetCodeRepository(
                this.SharedContext,
                this.Logger,
                new CachingManagerFactory(this.Cache),
                new PagingStrategyFactory(),
                new OrderingStrategyFactory());
        }
    }
}