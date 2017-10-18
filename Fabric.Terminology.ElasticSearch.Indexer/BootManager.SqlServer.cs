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
            services.AddSingleton<TerminologySqlSettings>(
                factory => factory.GetService<IndexerConfiguration>().TerminologySqlSettings);

            services.AddSingleton<IMemoryCacheSettings>(
                factory => factory.GetService<TerminologySqlSettings>());

            if (this.configuration.TerminologySqlSettings.MemoryCacheEnabled)
            {
                services.AddSingleton<IMemoryCacheProvider, MemoryCacheProvider>();
            }
            else
            {
                services.AddSingleton<IMemoryCacheProvider, NullMemoryCacheProvider>();
            }

            services.AddSingleton<ICachingManagerFactory, CachingManagerFactory>();

            services.AddSingleton<SharedContextFactory>();
            services.AddSingleton<ClientTermContextFactory>();
            services.AddSingleton<IPagingStrategyFactory, PagingStrategyFactory>();

            services.AddTransient<Lazy<ClientTermContext>>(factory => factory.GetService<ClientTermContextFactory>().CreateLazy());

            services.AddTransient<IClientTermValueSetRepository, SqlClientTermValueSetRepository>();
            services.AddTransient<IValueSetCodeRepository, SqlValueSetCodeRepository>();
            services.AddTransient<IValueSetCodeCountRepository, SqlValueSetCodeCountRepository>();
            services.AddTransient<IValueSetBackingItemRepository, SqlValueSetBackingItemRepository>();
            services.AddTransient<ICodeSystemRepository, SqlCodeSystemRepository>();
            services.AddTransient<ICodeSystemCodeRepository, SqlCodeSystemCodeRepository>();

            services.AddTransient<SharedContext>(factory => factory.GetService<SharedContextFactory>().Create());
            services.AddTransient<IValueSetService, SqlValueSetService>();
            services.AddTransient<ICodeSystemService, SqlCodeSystemService>();
            services.AddTransient<ICodeSystemCodeService, SqlCodeSystemCodeService>();
        }

        private IndexerConfiguration LoadConfiguration()
        {
            var config = new ConfigurationBuilder().SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json")
                .Build();
            var settings = new IndexerConfiguration();
            config.Bind(settings);

            this.logger.Information("Completed loading appsettings.json");
            return settings;
        }
    }
}