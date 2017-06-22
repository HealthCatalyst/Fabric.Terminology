using System;
using Fabric.Terminology.API.Logging;
using Moq;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Fabric.Terminology.TestsBase.Fixtures
{
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
            Logger = EnableLogging ? LogFactory.CreateLogger(LoggingLevelSwitch) : new Mock<ILogger>().Object;
        }
    }
}