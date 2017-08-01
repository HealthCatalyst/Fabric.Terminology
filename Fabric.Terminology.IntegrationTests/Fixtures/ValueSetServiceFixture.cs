namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.Domain.Strategy;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence;

    public class ValueSetServiceFixture : RepositoryFixtureBase
    {
        public ValueSetServiceFixture()
        {
            this.Initialize();
        }

        public IValueSetService ValueSetService { get; private set; }

        private void Initialize()
        {
            var valueSetCodeRepository = new SqlValueSetCodeRepository(
                this.SharedContext,
                this.ClientTermContext.AsLazy(),
                this.Logger,
                new DefaultPagingStrategy<ValueSetCodeDto, IValueSetCode>(100));

            var valueSetRepository = new SqlValueSetRepository(
                this.SharedContext,
                this.ClientTermContext.AsLazy(),
                this.Cache,
                this.Logger,
                valueSetCodeRepository,
                new DefaultPagingStrategy<ValueSetDescriptionDto, IValueSet>(20));

            this.ValueSetService = new ValueSetService(valueSetRepository, new ValueSetValidationStrategy());
        }
    }
}