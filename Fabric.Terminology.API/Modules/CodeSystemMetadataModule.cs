namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Collections.Generic;

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
            modelCatalog.AddModel<CodeSystemApiModel>();
            modelCatalog.AddModel<MultipleCodeSystemQuery>();
            modelCatalog.AddModel<Guid>();

            this.RouteDescriber.DescribeRouteWithParams(
                "GetAll",
                "Returns the collection of all available code systems",
                "Gets a collection of all available code systems",
                new[]
                {
                    new HttpResponseMetadata<IEnumerable<CodeSystemApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new Parameter[] { },
                new[] { TagsFactory.GetCodeSystemTag() });

            this.RouteDescriber.DescribeRouteWithParams(
                "GetCodeSystem",
                "Returns a code system by it's CodeSystemGuid (key)",
                "Gets a CodeSystem by it's CodeSystemGuid",
                new[]
                {
                    new HttpResponseMetadata<CodeSystemApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 404, Message = "Not Found" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[] { ParameterFactory.GetCodeSystemGuid() },
                new[] { TagsFactory.GetCodeSystemTag() });

            this.RouteDescriber.DescribeRouteWithParams(
                "GetCodeSystems",
                "Gets multiple CodeSystems",
                "Gets an array of CodeSystems matching the CodeSystemGuid collection",
                new[]
                {
                    new HttpResponseMetadata<IEnumerable<CodeSystemApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    new BodyParameter<MultipleCodeSystemQuery>(modelCatalog) { Required = true, Name = "Model" }
                },
                new[] { TagsFactory.GetCodeSystemTag() });
        }
    }
}
