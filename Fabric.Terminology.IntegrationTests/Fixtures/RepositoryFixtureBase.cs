namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using System;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.TestsBase.Fixtures;

    public abstract class RepositoryFixtureBase : AppConfigurationFixture
    {
        protected RepositoryFixtureBase()
        {
            var factory = new SharedContextFactory(this.AppConfiguration.TerminologySqlSettings, this.Logger);
            this.SharedContext = factory.Create();
            if (this.SharedContext.IsInMemory)
            {
                throw new InvalidOperationException();
            }

            this.Cache = this.AppConfiguration.TerminologySqlSettings.MemoryCacheEnabled ?
                (IMemoryCacheProvider)new MemoryCacheProvider(this.AppConfiguration.TerminologySqlSettings) :
                new NullMemoryCacheProvider(this.AppConfiguration.TerminologySqlSettings);
        }

        internal SharedContext SharedContext { get; }

        protected override bool EnableLogging => true;

        protected IMemoryCacheProvider Cache { get; }
    }
}
