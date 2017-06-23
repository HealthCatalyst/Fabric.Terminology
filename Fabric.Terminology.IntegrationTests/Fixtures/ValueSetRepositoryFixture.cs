namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using System;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Persistence;

    public class ValueSetRepositoryFixture : RepositoryFixtureBase, IDisposable
    {
        public ValueSetRepositoryFixture()
        {
            this.Initialize();
        }

        public IValueSetRepository ValueSetRepository { get; private set; }

        private void Initialize()
        {
            var valueSetCodeRepository = new SqlValueSetCodeRepository(this.SharedContext, this.Logger);
            this.ValueSetRepository = new SqlValueSetRespository(
                                        this.SharedContext,
                                        this.Cache,
                                        this.Logger,
                                        valueSetCodeRepository,
                                        this.AppConfiguration.ValueSetSettings);
        }
    }
}
