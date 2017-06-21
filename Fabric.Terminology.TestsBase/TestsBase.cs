using Fabric.Terminology.API.Logging;
using Moq;
using Serilog.Core;
using Serilog.Events;
using Xunit.Abstractions;
using ILogger = Serilog.ILogger;

namespace Fabric.Terminology.TestsBase
{
    public abstract class TestsBase
    {
        protected TestsBase(ITestOutputHelper output, ConfigTestFor testType = ConfigTestFor.Unit)
        {
            this.Output = output;
            this.TestType = testType;

            Logger = TestType == ConfigTestFor.Unit ? new Mock<ILogger>().Object : LogFactory.CreateLogger(new LoggingLevelSwitch(LogEventLevel.Verbose)); 
        }

        protected ITestOutputHelper Output { get; }

        protected ConfigTestFor TestType { get; }

        protected ILogger Logger { get; }
    }
}