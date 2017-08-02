namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Strategy;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence;

    public class CustomValueSetRepositoryFixture : RepositoryFixtureBase
    {
        public CustomValueSetRepositoryFixture()
        {
            this.Initialize();
        }

        //// not using interface for more direct access to testing internals
        internal SqlValueSetRepository ValueSetRepository { get; private set; }

        protected override bool ClientTermContextInMemory => true;

        private void Initialize()
        {
            var valueSetCodeRepository = new SqlValueSetCodeRepository(
                this.SharedContext,
                this.ClientTermContext.AsLazy(),
                this.Logger,
                new DefaultPagingStrategy<ValueSetCodeDto, IValueSetCode>(100));

            this.ValueSetRepository = new SqlValueSetRepository(
                this.SharedContext,
                this.ClientTermContext.AsLazy(),
                this.Cache,
                this.Logger,
                valueSetCodeRepository,
                new DefaultPagingStrategy<ValueSetDescriptionDto, IValueSet>(20),
                new IdentifyIsCustomStrategy());
        }
    }
}