namespace Fabric.Terminology.API.Modules
{
    using System;

    using Fabric.Terminology.API.MetaData;
    using Fabric.Terminology.API.Models;

    using Nancy.Swagger;
    using Nancy.Swagger.Modules;
    using Nancy.Swagger.Services;
    using Nancy.Swagger.Services.RouteUtils;

    using Swagger.ObjectModel;

    public class CodeSystemMetadataModule : SwaggerMetadataModule
    {
        public CodeSystemMetadataModule(
            ISwaggerModelCatalog modelCatalog,
            ISwaggerTagCatalog tagCatalog)
            : base(modelCatalog, tagCatalog)
        {
            modelCatalog.AddModels(
                typeof(CodeSystemApiModel),
                typeof(Guid)
            );

            this.RouteDescriber.DescribeRouteWithParams(
                "GetAll",
                "Returns the collection of all available code systems",
                "Gets a collection of all available code systems",
                new[]
                {
                    new HttpResponseMetadata<CodeSystemApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new Parameter[] { },
                new[] { TagsFactory.GetCodeSystemTag() });
        }
    }
}
