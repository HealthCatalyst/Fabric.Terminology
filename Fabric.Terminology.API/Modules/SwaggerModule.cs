namespace Fabric.Terminology.API.Modules
{
    using Nancy;
    using Nancy.Swagger.Services;

    public sealed class SwaggerModule : NancyModule
    {
        public SwaggerModule(ISwaggerMetadataProvider converter)
            : base("/swagger/")
        {
            this.Get("/index", _ => this.GetSwaggerUrl());

            this.Get("/swagger.json", _ => converter.GetSwaggerJson(this.Context).ToJson());
        }

        private Response GetSwaggerUrl() =>
            this.Response.AsRedirect(
                $"index.html?url=swagger.json");
    }
}