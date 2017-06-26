namespace Fabric.Terminology.API
{
    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.DependencyInjection;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Configuration;

    using JetBrains.Annotations;

    using Nancy;
    using Nancy.TinyIoc;
    using Serilog;

    internal class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IAppConfiguration appConfig;
        private readonly ILogger logger;

        public Bootstrapper(IAppConfiguration config, ILogger log)
        {
            this.appConfig = config;
            this.logger = log;
        }

        protected override void ConfigureApplicationContainer([NotNull] TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IAppConfiguration>(this.appConfig);
            container.Register<IMemoryCacheSettings>(this.appConfig.TerminologySqlSettings);
            container.Register(this.appConfig.ValueSetSettings);
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

            // Persistence (Must precede service registration)
            container.ComposeFrom<SqlAppComposition>();
        }

        protected override void ConfigureRequestContainer([NotNull] TinyIoCContainer container, [NotNull] NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.ComposeFrom<SqlRequestComposition>();
            container.ComposeFrom<ServicesRequestComposition>();
        }
    }
}