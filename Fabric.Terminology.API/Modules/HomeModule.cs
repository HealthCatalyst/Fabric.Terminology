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
            this.Get("/", _ => this.GetSwaggerUrl());

            this.Get("/Ping", _ => HttpStatusCode.OK, null, "Ping");

            this.Get("/Version", _ => TerminologyVersion.SemanticVersion.ToString());
        }

        private Response GetSwaggerUrl()
        {
            var hostName = this.Request.Url.HostName;
            var port = this.Request.Url.Port ?? 80;
            var redirect = hostName.Contains("localhost") || string.IsNullOrWhiteSpace(this.appConfig.BaseTerminologyEndpoint)
                               ? $"http://{hostName}:{port}/swagger/index.html?url=http://{hostName}:{port}/api-docs"
                               : $"{this.appConfig.BaseTerminologyEndpoint}/swagger/index.html?url={this.appConfig.BaseTerminologyEndpoint}/api-docs";

            return this.Response.AsRedirect(redirect);
        }
    }
}