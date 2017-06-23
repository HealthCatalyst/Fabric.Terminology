namespace Fabric.Terminology.API.Logging
{
    using Serilog;
    using Serilog.Core;

    /// <summary>
    /// Responsible for creating the <see cref="ILogger"/>
    /// </summary>
    /// <remarks>
    /// This class should be removed with Fabric.Platform logging has been finalized.
    /// </remarks>
    internal class LogFactory
    {
        public static ILogger CreateLogger(LoggingLevelSwitch levelSwitch)
        {
            // TODO work on log format
            return new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole()
                .WriteTo.RollingFile("Logs\\fabric-terminology-{Date}.txt")
                .CreateLogger();
        }
    }
}