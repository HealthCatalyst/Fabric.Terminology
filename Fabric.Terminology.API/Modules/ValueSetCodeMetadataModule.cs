namespace Fabric.Terminology.API.Modules
{
    using System.Collections.Generic;

    using Fabric.Terminology.API.MetaData;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;

    using Nancy.Swagger;
    using Nancy.Swagger.Modules;
    using Nancy.Swagger.Services;
    using Nancy.Swagger.Services.RouteUtils;

    public class ValueSetCodeMetadataModule : SwaggerMetadataModule
    {
        public ValueSetCodeMetadataModule(
            ISwaggerModelCatalog modelCatalog,
            ISwaggerTagCatalog tagCatalog)
            : base(modelCatalog, tagCatalog)
        {
            modelCatalog.AddModel<PagedCollection<ValueSetCodeApiModel>>();

            this.RouteDescriber.DescribeRouteWithParams(
                "GetAllValueSetCodesPaged",
                "Returns a paged list of value set codes",
                "Gets a paged collection of value set codes",
                new[]
                {
                    new HttpResponseMetadata<PagedCollection<ValueSetCodeApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    ParameterFactory.GetSkip(),
                    ParameterFactory.GetTop(),
                    ParameterFactory.GetCodeSystemGuidsArray()
                },
                new[] { TagsFactory.GetValueSetCodeTag() });

            this.RouteDescriber.DescribeRouteWithParams(
                "GetValueSetCodes",
                "Returns a list of value set codes for a given a CodeGuid",
                "Gets a list of value set codes for a given a CodeGuid",
                new[]
                {
                    new HttpResponseMetadata<IEnumerable<ValueSetCodeApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    ParameterFactory.GetCodeGuid()
                },
                new[]
                {
                    TagsFactory.GetValueSetCodeTag()
                });

            this.RouteDescriber.DescribeRouteWithParams(
                "GetValueSetCodePagedByValueSet",
                "Returns a paged list of value set codes",
                "Gets a paged collection of value set codes",
                new[]
                {
                    new HttpResponseMetadata<PagedCollection<ValueSetCodeApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    ParameterFactory.GetValueSetGuid(),
                    ParameterFactory.GetSkip(),
                    ParameterFactory.GetTop(),
                    ParameterFactory.GetCodeSystemGuidsArray()
                },
                new[] { TagsFactory.GetValueSetCodeTag() });
        }
    }
}
