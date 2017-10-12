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

    public class CodeSystemCodeMetadataModule : SwaggerMetadataModule
    {
        public CodeSystemCodeMetadataModule(
            ISwaggerModelCatalog modelCatalog,
            ISwaggerTagCatalog tagCatalog)
            : base(modelCatalog, tagCatalog)
        {
            modelCatalog.AddModel<CodeSystemCodeApiModel>();
            modelCatalog.AddModel<Guid>();

            this.RouteDescriber.DescribeRouteWithParams(
                "GetCodeSystemCode",
                "Returns a code system code by it's CodeGuid (key)",
                "Gets a CodeSystemCode by it's CodeGuid",
                new[]
                {
                    new HttpResponseMetadata<CodeSystemCodeApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 404, Message = "Not Found" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[] { ParameterFactory.GetCodeGuid() },
                new[] { TagsFactory.GetCodeSystemCodeTag() });
        }
    }
}
