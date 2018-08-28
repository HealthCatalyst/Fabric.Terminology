namespace Fabric.Terminology.API.Modules
{
    using Fabric.Terminology.API.Configuration;

    using Nancy;
    using Nancy.Extensions;

    public sealed class HomeModule : NancyModule
    {
        public HomeModule()
        {
            this.Get("/", _ => this.Redirect(), null, "Redirect");

            this.Get("/Ping", _ => HttpStatusCode.OK, null, "Ping");

            this.Get("/Version", _ => TerminologyVersion.SemanticVersion.ToString());
        }

        private dynamic Redirect() => this.Response.AsRedirect(this.Context.ToFullPath("~/swagger/index"));
    }
}