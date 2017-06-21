using System;
using Fabric.Terminology.API.Logging;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Persistence;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Fabric.Terminology.TestsBase;
using Serilog.Core;
using Serilog.Events;

namespace Fabric.Terminology.IntegrationTests.Repositories
{
    public class ValueSetCodeRepositoryFixture : IDisposable
    {
        public ValueSetCodeRepositoryFixture()
        {
            var appConfig = TestHelper.GetAppConfig();
            var logger = LogFactory.CreateLogger(new LoggingLevelSwitch(LogEventLevel.Verbose));
            var factory = new SharedContextFactory(appConfig.TerminologySqlSettings, logger);
            var context = factory.Create();
            if (context.IsInMemory) throw new InvalidOperationException();
            ValueSetCodeRepository = new SqlValueSetCodeRepository(factory.Create(), new MemoryCacheProvider());
        }

        public IValueSetCodeRepository ValueSetCodeRepository { get; }

        public void Dispose()
        {
            // nothing to do here thus far
        }
    }
}