namespace Fabric.Terminology.API.Logging
{
    using System.IO;

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
            var logsPath = Path.Combine("Logs", "fabric-terminology-{Date}.txt");
            return new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole()
                .WriteTo.RollingFile(logsPath)
                .CreateLogger();
        }
    }
}