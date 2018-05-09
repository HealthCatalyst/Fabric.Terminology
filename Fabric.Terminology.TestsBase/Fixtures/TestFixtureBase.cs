namespace Fabric.Terminology.TestsBase.Fixtures
{
    using System;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Logging;
    using Moq;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;

#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public abstract class TestFixtureBase : IDisposable
    {
        protected TestFixtureBase()
        {
            this.Initialize();
        }

        public ILogger Logger { get; private set; }

        protected virtual LoggingLevelSwitch LoggingLevelSwitch => new LoggingLevelSwitch(LogEventLevel.Verbose);

        protected virtual bool EnableLogging => false;

        public virtual void Dispose()
        {
        }

        private void Initialize()
        {
            this.Logger = this.EnableLogging ? LogFactory.CreateTraceLogger(new LoggingLevelSwitch(), new ApplicationInsightsSettings { Enabled = false }) : new Mock<ILogger>().Object;
        }
    }
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
#pragma warning restore CA1063 // Implement IDisposable Correctly
}