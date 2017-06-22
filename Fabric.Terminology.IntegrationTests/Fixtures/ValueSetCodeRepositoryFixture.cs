using System;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Persistence;

namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    public class ValueSetCodeRepositoryFixture : RepositoryFixtureBase
    {
        public ValueSetCodeRepositoryFixture()
        {
            this.Initialize();
        }

        public IValueSetCodeRepository ValueSetCodeRepository { get; private set; }

        private void Initialize()
        {
            ValueSetCodeRepository = new SqlValueSetCodeRepository(SharedContext, Logger, Cache);
        }
    }
}