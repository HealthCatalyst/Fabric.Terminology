namespace Fabric.Terminology.API
{
    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.DependencyInjection;
    using Fabric.Terminology.API.Validators;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Configuration;

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
            container.Register<IClientTermCacheManager, ClientTermCacheManager>().AsSingleton();

            container.Register<IValueSetStatusChangePolicy, DefaultValueSetStatusChangePolicy>().AsSingleton();

            // Persistence (Must precede service registration)
            container.ComposeFrom<SqlAppComposition>();
        }

        protected override void ConfigureRequestContainer(
            [NotNull] TinyIoCContainer container,
            [NotNull] NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.ComposeFrom<SqlRequestComposition>();
            container.ComposeFrom<ServicesRequestComposition>();
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