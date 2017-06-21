using System;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Persistence;

namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    public class ValueSetCodeRepositoryFixture : RepositoryFixtureBase, IDisposable
    {
        public ValueSetCodeRepositoryFixture()
        {
            this.Initialize();
        }

        public IValueSetCodeRepository ValueSetCodeRepository { get; private set; }

        public void Dispose()
        {
            // nothing to do here thus far
        }

        private void Initialize()
        {
            ValueSetCodeRepository = new SqlValueSetCodeRepository(SharedContext, Logger, Cache);
        }
    }
}