namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using System;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.TestsBase.Fixtures;

    public class ValueSetRepositoryFixture : RepositoryFixtureBase, IDisposable
    {
        public ValueSetRepositoryFixture()
        {
            this.Initialize();
        }

        public IValueSetRepository ValueSetRepository { get; private set; }

        private void Initialize()
        {
            var valueSetCodeRepository = new SqlValueSetCodeRepository(
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
        }
    }
}