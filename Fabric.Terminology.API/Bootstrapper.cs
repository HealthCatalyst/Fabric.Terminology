using System;
using Fabric.Terminology.API.Configuration;
using Fabric.Terminology.API.DependencyInjection;
using Fabric.Terminology.API.Logging;
using Fabric.Terminology.Domain;
using Fabric.Terminology.SqlServer.Caching;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Fabric.Terminology.API
{
    internal class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IAppConfiguration _appConfig;
        private readonly ILogger _logger;

        public Bootstrapper(IAppConfiguration appConfig, ILogger logger)
        {
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }


        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
          
            // TODO add security
            //var owinEnvironment = context.GetOwinEnvironment();
            //var principal = owinEnvironment["owin.RequestUser"] as ClaimsPrincipal;
            //context.CurrentUser = principal;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IAppConfiguration>(_appConfig);
            container.Register<ILogger>(_logger);

            // Caching
            if (_appConfig.TerminologySqlSettings.MemoryCacheEnabled)
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

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.ComposeFrom<SqlRequestComposition>();
            container.ComposeFrom<ServicesRequestComposition>();
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // TODO error handling and logging setup
        }
    }
}