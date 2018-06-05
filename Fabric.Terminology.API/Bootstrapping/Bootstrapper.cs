namespace Fabric.Terminology.API.Bootstrapping
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices.ComTypes;

    using Catalyst.DosApi.Discovery;
    using Catalyst.DosApi.Discovery.Catalyst.DiscoveryService.Models;

    using Fabric.Terminology.API.Bootstrapping.PipelineHooks;
    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Constants;
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
    using Swagger.ObjectModel.Builders;

    internal class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IAppConfiguration appConfig;

        private readonly ILogger logger;

        private readonly IDiscoveryServiceClient discoveryServiceClient;

        public Bootstrapper(
            IAppConfiguration config,
            ILogger log,
            IDiscoveryServiceClient discoveryServiceClient)
        {
            this.appConfig = config;
            this.logger = log;
            this.discoveryServiceClient = discoveryServiceClient;
        }

        protected override void ApplicationStartup([NotNull] TinyIoCContainer container, [NotNull] IPipelines pipelines)
        {
            this.InitializeSwaggerMetadata();

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

            pipelines.BeforeRequest += ctx => RequestHooks.RemoveContentTypeHeaderForGet(ctx);
            pipelines.BeforeRequest += ctx => RequestHooks.ErrorResponseIfContentTypeMissingForPostPutAndPatch(ctx);

            pipelines.AfterRequest += ctx =>
                {
                    foreach (var corsHeader in HttpResponseHeaders.CorsHeaders)
                    {
                        ctx.Response.Headers.Add(corsHeader.Item1, corsHeader.Item2);
                    }
                };
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
            container.Register<IDiscoveryServiceClient>(this.discoveryServiceClient);
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

            container.Register<IValueSetUpdateValidationPolicy, DefaultValueSetUpdateValidationPolicy>().AsSingleton();

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
            container.Register<ValueSetValidatorCollection>();
        }

        private void InitializeSwaggerMetadata()
        {
            // This seems like a redundant call but the api service is internally "statically" cached so it's essentially
            // just a lookup
            var authority = this.discoveryServiceClient.GetApiServiceForIdentityFromConfig(this.appConfig);

            SwaggerMetadataProvider.SetInfo(
                "Shared Terminology Data Services",
                TerminologyVersion.SemanticVersion.ToString(),
                "Shared Terminology Data Services - Fabric.Terminology.API",
                new Contact() { EmailAddress = "terminology-api@healthcatalyst.com" });

            var securitySchemeBuilder = new Oauth2SecuritySchemeBuilder();
            securitySchemeBuilder.Flow(Oauth2Flows.Implicit);
            securitySchemeBuilder.Description("Authentication with Fabric.Identity");
            securitySchemeBuilder.AuthorizationUrl(authority.AbsoluteUri);
            securitySchemeBuilder.Scope("fabric/terminology.read", "Grants read access to fabric.terminology resources.");
            securitySchemeBuilder.Scope("fabric/terminology.write", "Grants write access to fabric.terminology resources.");
            try
            {
                SwaggerMetadataProvider.SetSecuritySchemeBuilder(securitySchemeBuilder, "fabric.identity");
            }
            catch (ArgumentException ex)
            {
                this.logger.Warning("Error configuring Swagger Security Scheme. {ExceptionMessage}", ex.Message);
            }
            catch (NullReferenceException ex)
            {
                this.logger.Warning("Error configuring Swagger Security Scheme: {ExceptionMessage}", ex.Message);
            }
        }
    }
}