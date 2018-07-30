namespace Fabric.Terminology.TestsBase.Fixtures
{
    using System;

    using Catalyst.Infrastructure.Caching;

    using Fabric.Terminology.SqlServer;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.TestsBase.Seeding;

    public abstract class RepositoryFixtureBase : AppConfigurationFixture
    {
        protected RepositoryFixtureBase()
        {
            this.Cache = this.AppConfiguration.TerminologySqlSettings.MemoryCacheEnabled ?
                (IMemoryCacheProvider)new MemoryCacheProvider(this.AppConfiguration.TerminologySqlSettings.AsMemoryCacheProviderSettings()) :
                new NullMemoryCachingProvider();

            this.Initialize();
        }

        internal SharedContext SharedContext { get; private set; }

        internal ClientTermContext ClientTermContext { get; private set; }

        protected override bool EnableLogging => true;

        protected virtual bool SharedContextInMemory => false;

        protected virtual bool ClientTermContextInMemory => false;

        protected IMemoryCacheProvider Cache { get; }

        private void Initialize()
        {
            var factory = new SharedContextFactory(this.AppConfiguration.TerminologySqlSettings, this.Logger);

            var clientTermFactory = this.ClientTermContextInMemory
                                        ? new ClientTermContextFactory(
                                            this.AppConfiguration.TerminologySqlSettings,
                                            this.Logger,
                                            new ClientTermSeededDatabaseInitializer())
                                        : new ClientTermContextFactory(
                                            this.AppConfiguration.TerminologySqlSettings,
                                            this.Logger);

            this.SharedContext = factory.Create(this.SharedContextInMemory);
            this.ClientTermContext = clientTermFactory.Create(this.ClientTermContextInMemory);

            if (this.SharedContext.IsInMemory)
            {
                throw new InvalidOperationException("Cannot use in memory tests with the SharedContext");
            }
        }
    }
}
