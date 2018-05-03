namespace Fabric.Terminology.API.Modules
{
    using Fabric.Terminology.API.Configuration;

    using Nancy;

    public sealed class HomeModule : NancyModule
    {
        public HomeModule()
        {
            this.Get("/", _ => this.GetSwaggerUrl());

            this.Get("/Ping", _ => HttpStatusCode.OK, null, "Ping");

            this.Get("/Version", _ => TerminologyVersion.SemanticVersion.ToString());
        }

        private Response GetSwaggerUrl()
        {
            var hostName = this.Request.Url.HostName;
            var port = this.Request.Url.Port ?? 80;
            return this.Response.AsRedirect($"http://{hostName}:{port}/swagger/index.html?url=http://{hostName}:{port}/api-docs");
        }
    }
}