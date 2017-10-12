namespace Fabric.Terminology.API
{
    using System;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.DependencyInjection;
    using Fabric.Terminology.API.Validators;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.ElasticSearch.Configuration;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Configuration;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Services;

    using JetBrains.Annotations;

    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Conventions;
    using Nancy.Swagger.Services;
    using Nancy.TinyIoc;

    using Serilog;

    using Swagger.ObjectModel;

    internal class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IAppConfiguration appConfig;

        private readonly ILogger logger;

        public Bootstrapper(IAppConfiguration config, ILogger log)
        {
            this.appConfig = config;
            this.logger = log;
        }

        protected override void ApplicationStartup([NotNull] TinyIoCContainer container, [NotNull] IPipelines pipelines)
        {
            SwaggerMetadataProvider.SetInfo(
                "Shared Terminology Data Services",
                TerminologyVersion.SemanticVersion.ToString(),
                "Shared Terminology Data Services - Fabric.Terminology.API",
                new Contact() { EmailAddress = "terminology-api@healthcatalyst.com" });

            base.ApplicationStartup(container, pipelines);

            pipelines.OnError.AddItemToEndOfPipeline(
                (ctx, ex) =>
                    {
                        this.logger.Error(
                            ex,
                            "Unhandled error on request: @{Url}. Error Message: @{Message}",
                            ctx.Request.Url,
                            ex.Message);
                        return ctx.Response;
                    });
        }

        protected override void ConfigureConventions([NotNull] NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.AddDirectory("/swagger");
        }

        protected override void ConfigureApplicationContainer([NotNull] TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IAppConfiguration>(this.appConfig);
            container.Register<IMemoryCacheSettings>(this.appConfig.TerminologySqlSettings);
            container.Register<TerminologySqlSettings>((c, s) => c.Resolve<IAppConfiguration>().TerminologySqlSettings);
            container.Register<ElasticSearchSettings>((c, s) => c.Resolve<IAppConfiguration>().ElasticSearchSettings);

            container.Register<ILogger>(this.logger);

            // Caching
            if (this.appConfig.TerminologySqlSettings.MemoryCacheEnabled)
            {
                container.Register<IMemoryCacheProvider, MemoryCacheProvider>().AsSingleton();
            }
            else
            {
                container.Register<IMemoryCacheProvider, NullMemoryCacheProvider>().AsSingleton();
            }

            container.Register<ICachingManagerFactory, CachingManagerFactory>().AsSingleton();
            container.Register<ICodeSystemCachingManager, CodeSystemCachingManager>().AsSingleton();
            container.Register<ICodeSystemCodeCachingManager, CodeSystemCodeCachingManager>().AsSingleton();

            // Persistence (Must precede service registration)
            container.Register<IPagingStrategyFactory, PagingStrategyFactory>().AsSingleton();
            container.ComposeFrom<SqlAppComposition>();
            if (this.appConfig.ElasticSearchSettings.Enabled)
            {
                container.ComposeFrom<ElasticSearchAppComposition>();
            }
        }

        protected override void ConfigureRequestContainer(
            [NotNull] TinyIoCContainer container,
            [NotNull] NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.Register<SharedContext>((c, p) => c.Resolve<SharedContextFactory>().Create());
            container.Register<Lazy<ClientTermContext>>((c, p) => c.Resolve<ClientTermContextFactory>().CreateLazy());
            container.Register<IValueSetCodeRepository, SqlValueSetCodeRepository>();
            container.Register<IValueSetCodeCountRepository, SqlValueSetCodeCountRepository>();
            container.Register<IValueSetBackingItemRepository, SqlValueSetBackingItemRepository>();

            // ClientTerm services are always SQL
            container.Register<IClientTermValueSetRepository, SqlClientTermValueSetRepository>();
            container.Register<IClientTermValueSetService, SqlClientTermValueSetService>().AsSingleton();

            container.Register<ICodeSystemRepository, SqlCodeSystemRepository>();
            container.Register<ICodeSystemCodeRepository, SqlCodeSystemCodeRepository>();

            if (!this.appConfig.ElasticSearchSettings.Enabled)
            {
                container.ComposeFrom<SqlServicesComposition>();
            }

            container.Register<ICodeSystemService, SqlCodeSystemService>().AsSingleton();
            container.Register<ICodeSystemCodeService, SqlCodeSystemCodeService>().AsSingleton();

            container.Register<ValueSetValidator>();
        }

        protected override void RequestStartup(
            [NotNull] TinyIoCContainer container,
            [NotNull] IPipelines pipelines,
            [NotNull] NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            pipelines.AfterRequest.AddItemToEndOfPipeline(
                x => x.Response.Headers.Add("Access-Control-Allow-Origin", "*"));
        }
    }
}