using System;
using System.Security.Claims;
using Fabric.Terminology.API.Configuration;
using Fabric.Terminology.Domain.Configuration;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Owin;
using Nancy.TinyIoc;

namespace Fabric.Terminology.API
{
    internal class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IAppConfiguration _appConfig;

        public Bootstrapper(IAppConfiguration appConfig)
        {
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            //var owinEnvironment = context.GetOwinEnvironment();
            //var principal = owinEnvironment["owin.RequestUser"] as ClaimsPrincipal;
            //context.CurrentUser = principal;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            container.Register(_appConfig);
            container.Register<TerminologySettings>(_appConfig.TerminologySettings);

        }
    }
}