using System;
using System.Collections.Generic;
using System.Text;
using Fabric.Terminology.API.Configuration;
using Fabric.Terminology.API.Logging;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Fabric.Terminology.TestsBase;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    public abstract class RepositoryFixtureBase
    {
        protected RepositoryFixtureBase()
        {
            AppConfig = TestHelper.GetAppConfig();
            Logger = LogFactory.CreateLogger(new LoggingLevelSwitch(LogEventLevel.Verbose));
            var factory = new SharedContextFactory(AppConfig.TerminologySqlSettings, Logger);
            SharedContext = factory.Create();
            if (SharedContext.IsInMemory) throw new InvalidOperationException();

            Cache = AppConfig.TerminologySqlSettings.MemoryCacheEnabled ?
                (IMemoryCacheProvider)new MemoryCacheProvider() :
                new NullMemoryCacheProvider();
        }

        internal SharedContext SharedContext { get; }

        protected ILogger Logger { get; }

        protected IAppConfiguration AppConfig { get; }

        
        protected IMemoryCacheProvider Cache { get; }
    }
}
