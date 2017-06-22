using System;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Fabric.Terminology.TestsBase.Fixtures;

namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    public abstract class RepositoryFixtureBase : AppConfigurationFixture
    {
        protected RepositoryFixtureBase()
        {
            var factory = new SharedContextFactory(AppConfiguration.TerminologySqlSettings, Logger);
            SharedContext = factory.Create();
            if (SharedContext.IsInMemory) throw new InvalidOperationException();

            Cache = AppConfiguration.TerminologySqlSettings.MemoryCacheEnabled ?
                (IMemoryCacheProvider)new MemoryCacheProvider(AppConfiguration.TerminologySqlSettings) :
                new NullMemoryCacheProvider(AppConfiguration.TerminologySqlSettings);
        }

        internal SharedContext SharedContext { get; }
        
        protected IMemoryCacheProvider Cache { get; }
    }
}
