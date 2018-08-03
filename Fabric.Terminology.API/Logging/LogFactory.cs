namespace Fabric.Terminology.API.Logging
{
    using System.IO;

    using Fabric.Terminology.API.Configuration;

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
        public static ILogger CreateTraceLogger(LoggingLevelSwitch levelSwitch, ApplicationInsightsSettings appInsightsConfig)
        {
            var loggerConfiguration = CreateLoggerConfiguration(levelSwitch);

            if (appInsightsConfig != null && appInsightsConfig.Enabled &&
                !string.IsNullOrEmpty(appInsightsConfig.InstrumentationKey))
            {
                loggerConfiguration.WriteTo.ApplicationInsightsTraces(appInsightsConfig.InstrumentationKey);
            }

            return loggerConfiguration.CreateLogger();
        }

        public static ILogger CreateEventLogger(LoggingLevelSwitch levelSwitch, ApplicationInsightsSettings appInsightsConfig)
        {
            var loggerConfiguration = CreateLoggerConfiguration(levelSwitch);

            if (appInsightsConfig != null && appInsightsConfig.Enabled &&
                !string.IsNullOrEmpty(appInsightsConfig.InstrumentationKey))
            {
                loggerConfiguration.WriteTo.ApplicationInsightsEvents(appInsightsConfig.InstrumentationKey);
            }

            return loggerConfiguration.CreateLogger();
        }

        private static LoggerConfiguration CreateLoggerConfiguration(LoggingLevelSwitch levelSwitch) =>
            new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.FromLogContext()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile(Path.Combine("Logs", "fabric-terminology-{Date}.txt"));
    }
}