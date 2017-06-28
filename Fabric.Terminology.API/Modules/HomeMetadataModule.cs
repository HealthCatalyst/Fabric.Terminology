namespace Fabric.Terminology.API.Modules
{
    using Nancy.Metadata.Modules;
    using Nancy.Swagger;

    using Swagger.ObjectModel;

    public class HomeMetadataModule : MetadataModule<PathItem>
    {
        public HomeMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            this.Describe["Ping"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.OperationId("Ping")
                        .Tag("BaseApi")
                        .Summary("Ping")
                        .Response(r => r.Description("OK"))));
        }
    }
}