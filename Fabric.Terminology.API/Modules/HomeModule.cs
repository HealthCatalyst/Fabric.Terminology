namespace Fabric.Terminology.API.Modules
{
    using Fabric.Terminology.API.Configuration;

    using Nancy;

    public sealed class HomeModule : NancyModule
    {
        private readonly IAppConfiguration appConfig;

        public HomeModule(IAppConfiguration appConfig)
        {
            this.appConfig = appConfig;
            this.Get("/", _ => this.Redirect(), null, "Redirect");

            this.Get("/Ping", _ => HttpStatusCode.OK, null, "Ping");

            this.Get("/Version", _ => TerminologyVersion.SemanticVersion.ToString());
        }

        private dynamic Redirect() => this.Response.AsRedirect($"{this.appConfig.SwaggerRootBasePath}/swagger/index");
    }
}