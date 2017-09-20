namespace Fabric.Terminology.TestsBase.Fixtures
{
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Persistence;

    public class CustomValueSetFixture : RepositoryFixtureBase
    {
        public CustomValueSetFixture()
        {
            this.Initialize();
        }

        internal SqlValueSetCodeRepository ValueSetCodeRepository { get; private set; }

        //// not using interface for more direct access to testing internals
        internal SqlValueSetRepository ValueSetRepository { get; private set; }

        internal IValueSetService ValueSetService { get; private set; }

        protected override bool ClientTermContextInMemory => true;

        private void Initialize()
        {
            this.ValueSetCodeRepository = new SqlValueSetCodeRepository(
                this.SharedContext,
                this.ClientTermContext.AsLazy(),
                this.Logger,
                new PagingStrategyFactory());

            this.ValueSetRepository = new SqlValueSetRepository(
                this.SharedContext,
                this.ClientTermContext.AsLazy(),
                this.Cache,
                this.Logger,
                new PagingStrategyFactory());

            this.ValueSetService = new ValueSetService(this.ValueSetRepository);
        }
    }
}