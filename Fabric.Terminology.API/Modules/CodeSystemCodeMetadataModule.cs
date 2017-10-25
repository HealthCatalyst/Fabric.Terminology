namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.API.MetaData;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Configuration;

    using Nancy.Swagger;
    using Nancy.Swagger.Modules;
    using Nancy.Swagger.Services;
    using Nancy.Swagger.Services.RouteUtils;

    public class CodeSystemCodeMetadataModule : SwaggerMetadataModule
    {
        public CodeSystemCodeMetadataModule(
            ISwaggerModelCatalog modelCatalog,
            ISwaggerTagCatalog tagCatalog,
            TerminologySqlSettings settings)
            : base(modelCatalog, tagCatalog)
        {
            modelCatalog.AddModel<CodeSystemCodeApiModel>();
            modelCatalog.AddModel<PagedCollection<CodeSystemCodeApiModel>>();
            modelCatalog.AddModel<MultipleCodeSystemCodeQuery>();
            modelCatalog.AddModel<FindByTermQuery>();
            modelCatalog.AddModel<BatchCodeQuery>();
            modelCatalog.AddModel<BatchCodeResultApiModel>();
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

            this.RouteDescriber.DescribeRouteWithParams(
                "GetPagedCodeSystemCodes",
                "Returns a paged collection of code system codes",
                "Gets a paged collection of code system codes",
                new[]
                {
                    new HttpResponseMetadata<PagedCollection<ValueSetApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    ParameterFactory.GetSkip(),
                    ParameterFactory.GetTop(settings.DefaultItemsPerPage),
                    ParameterFactory.GetCodeSystemGuidsArray()
                },
                new[] { TagsFactory.GetCodeSystemCodeTag() });

            this.RouteDescriber.DescribeRouteWithParams(
                "GetCodeSystemCodes",
                "Gets multiple code system codes",
                "Gets an array of code system codes matching the CodeGuid collection",
                new[]
                {
                    new HttpResponseMetadata<IEnumerable<CodeSystemCodeApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    new BodyParameter<MultipleCodeSystemCodeQuery>(modelCatalog) { Required = true, Name = "Model" }
                },
                new[] { TagsFactory.GetCodeSystemCodeTag() });

            this.RouteDescriber.DescribeRouteWithParams(
                "GetBatchCodes",
                "Gets a batch code system codes and non matching 'codes' given an array of 'codes', optionally constrained by code system",
                "Gets an array of code system codes matching the 'codes' collection and an array non matching 'codes', optionally constrained by code system",
                new[]
                {
                    new HttpResponseMetadata<BatchCodeResultApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 400, Message = "Bad Request" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    new BodyParameter<BatchCodeQuery>(modelCatalog) { Required = true, Name = "Model" }
                },
                new[] { TagsFactory.GetCodeSystemCodeTag() });

            this.RouteDescriber.DescribeRouteWithParams(
                "SearchCodeSystemCodes",
                "Search by 'Name' or 'Code' code system code operation",
                "Gets a paged collection of code system codes matching the 'Name' or 'Code' filter",
                new[]
                {
                    new HttpResponseMetadata<PagedCollection<CodeSystemCodeApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    new BodyParameter<FindByTermQuery>(modelCatalog) { Required = true, Name = "Model" }
                },
                new[] { TagsFactory.GetCodeSystemCodeTag() });
        }
    }
}
