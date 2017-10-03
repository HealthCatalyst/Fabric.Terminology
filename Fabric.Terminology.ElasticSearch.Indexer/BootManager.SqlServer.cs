namespace Fabric.Terminology.ElasticSearch.Indexer
{
    using System;
    using System.IO;

    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.ElasticSearch.Indexer.Configuration;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Configuration;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Services;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    internal partial class BootManager
    {
        private void RegisterSqlServices(IServiceCollection services)
        {
            var settings = this.LoadConfiguration();
            services.AddSingleton(settings);

            services.AddSingleton<IMemoryCacheSettings>(settings);

            if (settings.MemoryCacheEnabled)
            {
                services.AddSingleton<IMemoryCacheProvider, MemoryCacheProvider>();
            }
            else
            {
                services.AddSingleton<IMemoryCacheProvider, NullMemoryCacheProvider>();
            }

            services.AddSingleton<ICachingManagerFactory, CachingManagerFactory>();

            // TODO consider using AddDbContext<T>
            services.AddSingleton<SharedContextFactory>();
            services.AddSingleton<IPagingStrategyFactory, PagingStrategyFactory>();

            services.AddTransient<IValueSetCodeRepository, SqlValueSetCodeRepository>();
            services.AddTransient<IValueSetCodeCountRepository, SqlValueSetCodeCountRepository>();
            services.AddTransient<IValueSetBackingItemRepository, SqlValueSetBackingItemRepository>();

            services.AddTransient<SharedContext>(factory => factory.GetService<SharedContextFactory>().Create());
            services.AddTransient<IValueSetService, ValueSetService>();
        }

        private TerminologySqlSettings LoadConfiguration()
        {
            var config = new ConfigurationBuilder().SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json")
                .Build();
            var settings = new IndexerConfiguration();
            config.Bind(settings);

            this.logger.Information("Completed loading appsettings.json");
            return settings.TerminologySqlSettings;
        }
    }
}