namespace Fabric.Terminology.API.Bootstrapping
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using Catalyst.DosApi.Discovery;
    using Catalyst.Infrastructure.Caching;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Constants;
    using Fabric.Terminology.API.DependencyInjection;
    using Fabric.Terminology.API.Infrastructure;
    using Fabric.Terminology.API.Infrastructure.PipelineHooks;
    using Fabric.Terminology.API.Validators;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer;
    using Fabric.Terminology.SqlServer.Caching;

    using JetBrains.Annotations;

    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Conventions;
    using Nancy.Swagger.Services;
    using Nancy.TinyIoc;

    using Serilog;

    using Swagger.ObjectModel;
    using Swagger.ObjectModel.Builders;

    using IMemoryCacheProvider = Catalyst.Infrastructure.Caching.IMemoryCacheProvider;
    using MemoryCacheProvider = Catalyst.Infrastructure.Caching.MemoryCacheProvider;

    internal class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IAppConfiguration appConfig;

        private readonly ILogger logger;

        private readonly IDiscoveryServiceClient discoveryServiceClient;

        private readonly string rootPath;

        public Bootstrapper(
            IAppConfiguration config,
            ILogger log,
            IDiscoveryServiceClient discoveryServiceClient,
            string rootPath)
        {
            this.appConfig = config;
            this.logger = log;
            this.discoveryServiceClient = discoveryServiceClient;
            this.rootPath = rootPath;
        }

        /// <summary>
        /// Gets the <see cref="Uri"/> for the Identity Service
        /// </summary>
        private Lazy<Uri> IdentityServiceUri => new Lazy<Uri>(() => this.GetUriFromDiscovery(DiscoveryServicesKeys.Identity));

        /// <summary>
        /// Gets the <see cref="Uri"/> for the Authorization Service
        /// </summary>
        private Lazy<Uri> AuthorizationServiceUri => new Lazy<Uri>(() => this.GetUriFromDiscovery(DiscoveryServicesKeys.Authorization));

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
                    foreach (var corsHeader in HttpHeaderValues.CorsHeaders)
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
            container.Register<MemoryCacheProviderDefaultSettings>(this.appConfig.TerminologySqlSettings.AsMemoryCacheProviderSettings());
            container.Register<IDiscoveryServiceClient>(this.discoveryServiceClient);
            container.Register<ILogger>(this.logger);
            container.Register<AuthorizationServerSettings>(this.appConfig.AuthorizationServerSettings);

            // Caching
            if (this.appConfig.TerminologySqlSettings.MemoryCacheEnabled)
            {
                container.Register<IMemoryCacheProvider, MemoryCacheProvider>().AsSingleton();
            }
            else
            {
                container.Register<IMemoryCacheProvider, NullMemoryCachingProvider>().AsSingleton();
            }

            container.Register<ICachingManagerFactory, CachingManagerFactory>().AsSingleton();
            container.Register<ICodeSystemCachingManager, CodeSystemCachingManager>().AsSingleton();
            container.Register<ICodeSystemCodeCachingManager, CodeSystemCodeCachingManager>().AsSingleton();
            container.Register<IClientTermCacheManager, ClientTermCacheManager>().AsSingleton();

            container.Register<IValueSetUpdateValidationPolicy, DefaultValueSetUpdateValidationPolicy>().AsSingleton();

            // Persistence (Must precede service registration)
            container.ComposeFrom<SqlAppComposition>();

            var clientSettings = this.GetClientSettings();

            // Fabric.Identity and Authorization
            container.RegisterDosServices(
                this.appConfig.IdentityServerSettings,
                this.IdentityServiceUri.Value,
                this.AuthorizationServiceUri.Value,
                clientSettings);
        }

        protected override void ConfigureRequestContainer(
            [NotNull] TinyIoCContainer container,
            [NotNull] NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.Register<NancyContextWrapper>(new NancyContextWrapper(context));

            container.ComposeFrom<SqlRequestComposition>();
            container.ComposeFrom<ServicesRequestComposition>();
            container.Register<ValueSetValidatorCollection>();
        }

        private ClientSettings GetClientSettings()
        {
            var webConfigPath = Path.Combine(this.rootPath, "web.config");
            this.logger.Warning("PATH - " + webConfigPath);
            if (!File.Exists(webConfigPath))
            {
                throw new FileNotFoundException($"web.config not in {webConfigPath}");
            }

            var appSettings = XDocument.Load(webConfigPath)
                .Element("configuration")
                ?.Element("appSettings")
                ?.Elements("add")
                .ToDictionary(
                    appSetting => appSetting.Attribute("key")?.Value,
                    appSetting => appSetting.Attribute("value")?.Value);

            if (appSettings == null)
            {
                throw new InvalidOperationException("web.config is malformed or is missing appsettings");
            }

            var clientSettings = new ClientSettings
            {
                ClientId = appSettings.Where(keyvalue => keyvalue.Key == "ClientId")
                    .Select(keyvalue => keyvalue.Value)
                    .FirstOrDefault(),
                ClientSecret = appSettings.Where(keyvalue => keyvalue.Key == "ClientSecret")
                    .Select(keyvalue => keyvalue.Value)
                    .FirstOrDefault(),
                ApiSecret = appSettings.Where(keyvalue => keyvalue.Key == "apiSecret")
                    .Select(keyvalue => keyvalue.Value)
                    .FirstOrDefault(),
            };

            return clientSettings;
        }

        private void InitializeSwaggerMetadata()
        {
            SwaggerMetadataProvider.SetInfo(
                "Shared Terminology Data Services",
                TerminologyVersion.SemanticVersion.ToString(),
                "Shared Terminology Data Services - Fabric.Terminology.API",
                new Contact() { EmailAddress = "terminology-api@healthcatalyst.com" });

            var securitySchemeBuilder = new Oauth2SecuritySchemeBuilder();
            securitySchemeBuilder.Flow(Oauth2Flows.Implicit);
            securitySchemeBuilder.Description("Authentication with Fabric.Identity");
            securitySchemeBuilder.AuthorizationUrl(this.IdentityServiceUri.Value.AbsoluteUri);
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

        private Uri GetUriFromDiscovery(string serviceKey)
        {
            var task = this.discoveryServiceClient.RequestServiceUriByKeyAsync(serviceKey);
            task.Wait();
            return task.Result;
        }
    }
}
