using System;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Persistence;

namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    public class ValueSetRepositoryFixture : RepositoryFixtureBase, IDisposable
    {
        public ValueSetRepositoryFixture()
        {
            this.Initialize();
        }

        public IValueSetRepository ValueSetRepository { get; private set; }

        private void Initialize()
        {
            var valueSetCodeRepository = new SqlValueSetCodeRepository(SharedContext, Logger);
            ValueSetRepository = new SqlValueSetRespository(SharedContext, Cache, Logger, valueSetCodeRepository);
        }
    }
}
