namespace Fabric.Terminology.API.Modules
{
    using Fabric.Terminology.API.Configuration;

    using Nancy;

    public sealed class HomeModule : NancyModule
    {
        public HomeModule(IRootPathProvider pathProvider)
        {
            this.Get("/", _ => this.GetSwaggerUrl());

            this.Get("/Ping", _ => HttpStatusCode.OK, null, "Ping");

            this.Get("/Version", _ => TerminologyVersion.SemanticVersion.ToString());
        }

        private Response GetSwaggerUrl()
        {
            var port = this.Request.Url.Port ?? 80;
            return this.Response.AsRedirect($"http://localhost:{port}/swagger/index.html?url=http://localhost:{port}/api-docs");
        }
    }
}