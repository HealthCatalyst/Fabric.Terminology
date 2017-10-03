namespace Fabric.Terminology.ElasticSearch.Indexer
{
    using Fabric.Terminology.ElasticSearch.Logging;

    using Microsoft.Extensions.DependencyInjection;

    using Serilog;
    using Serilog.Core;

    internal partial class BootManager
    {
        private ILogger logger;

        public ServiceCollection Initialize()
        {
            var services = new ServiceCollection();

            this.logger = LogFactory.CreateLogger(new LoggingLevelSwitch());
            this.logger.Information("BootManager Initializing");
            services.AddSingleton(this.logger);

            // SqlServer
            this.RegisterSqlServices(services);

            // ElasticSearch
            this.RegisterNestServices(services);

            this.logger.Information("BootManager finished.");

            return services;
        }
    }
}