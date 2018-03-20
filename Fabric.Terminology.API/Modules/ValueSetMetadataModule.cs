namespace Fabric.Terminology.API.Modules
{
    using System;

    using Fabric.Terminology.API.MetaData;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Configuration;

    using Nancy.Swagger;
    using Nancy.Swagger.Modules;
    using Nancy.Swagger.Services;
    using Nancy.Swagger.Services.RouteUtils;

    public class ValueSetMetadataModule : SwaggerMetadataModule
    {
        public ValueSetMetadataModule(
            ISwaggerModelCatalog modelCatalog,
            ISwaggerTagCatalog tagCatalog,
            TerminologySqlSettings settings)
            : base(modelCatalog, tagCatalog)
        {
            modelCatalog.AddModels(
                typeof(ValueSetFindByTermQuery),
                typeof(PagedCollection<ValueSetApiModel>),
                typeof(PagedCollection<ValueSetItemApiModel>),
                typeof(PagerSettings),
                typeof(ValueSetApiModel),
                typeof(ValueSetItemApiModel),
                typeof(ValueSetCodeApiModel),
                typeof(ValueSetCodeCountApiModel),
                typeof(ClientTermValueSetApiModel),
                typeof(CodeOperation),
                typeof(ValueSetCopyApiModel),
                typeof(ValueSetStatus),
                typeof(CodeOperationSource),
                typeof(OperationInstruction),
                typeof(SortDirection),
                typeof(Guid)
                );

            // /{valueSetGuid}
            this.RouteDescriber.DescribeRouteWithParams(
                "GetValueSet",
                "Returns a ValueSet by it's ValueSetGuid",
                "Gets a ValueSet by it's ValueSetGuid",
                new[]
                {
                    new HttpResponseMetadata<ValueSetApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 400, Message = "Bad Request" },
                    new HttpResponseMetadata { Code = 404, Message = "Not Found" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    ParameterFactory.GetValueSetGuid(),
                    ParameterFactory.GetSummary(),
                    ParameterFactory.GetCodeSystemGuidsArray()
                },
                new[] { TagsFactory.GetValueSetTag() });

            this.RouteDescriber.DescribeRouteWithParams(
                "GetMultipleValueSets",
                "Gets multiple ValueSets",
                "Gets a collection of ValueSet's given a collection of ValueSetGuid(s)",
                new[]
                {
                    new HttpResponseMetadata<PagedCollection<ValueSetApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    new BodyParameter<MultipleValueSetsQuery>(modelCatalog) { Required = true, Name = "Model" }
                },
                new[] { TagsFactory.GetValueSetTag() });

            this.RouteDescriber.DescribeRouteWithParams(
                "GetPaged",
                "Returns a paged list of ValueSets",
                "Gets a paged collection of ValueSets",
                new[]
                {
                    new HttpResponseMetadata<PagedCollection<ValueSetApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    ParameterFactory.GetSkip(),
                    ParameterFactory.GetTop(settings.DefaultItemsPerPage),
                    ParameterFactory.GetSummary(),
                    ParameterFactory.GetCodeSystemGuidsArray(),
                    ParameterFactory.GetQueryStringStatusCode(),
                    ParameterFactory.GetOrderBy("Name")
                },
                new[] { TagsFactory.GetValueSetTag() });

            this.RouteDescriber.DescribeRouteWithParams(
                "GetValueSetVersions",
                "Gets all versions of a ValueSet",
                "Gets all versions of a ValueSet by it's published ID (Usually OID)",
                new[]
                {
                    new HttpResponseMetadata<ValueSetApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 404, Message = "Not Found" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[] { ParameterFactory.GetValueSetReferenceId(), ParameterFactory.GetSummary(), ParameterFactory.GetCodeSystemGuidsArray() },
                new[] { TagsFactory.GetValueSetTag() });

            this.RouteDescriber.DescribeRouteWithParams(
                "Search",
                "Search by 'Name' of ValueSet operation",
                "Gets a paged collection of ValueSet's matching the 'Name' filter",
                new[]
                {
                    new HttpResponseMetadata<PagedCollection<ValueSetApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    new BodyParameter<ValueSetFindByTermQuery>(modelCatalog) { Required = true, Name = "Model" }
                },
                new[] { TagsFactory.GetValueSetTag() });

            this.RouteDescriber.DescribeRouteWithParams(
                "AddValueSet",
                "Creates a new value set",
                "Creates a new value set",
                new[]
                {
                    new HttpResponseMetadata<ValueSetApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 400, Message = "Bad Request" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    new BodyParameter<ClientTermValueSetApiModel>(modelCatalog) { Required = true, Name = "Model" }
                },
                new[]
                {
                    TagsFactory.GetValueSetTag()
                });

            this.RouteDescriber.DescribeRouteWithParams(
                "PatchValueSet",
                "Updates a value set",
                "Updates a value set",
                new[]
                {
                    new HttpResponseMetadata<ValueSetApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 400, Message = "Bad Request" },
                    new HttpResponseMetadata { Code = 404, Message = "Not Found" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    ParameterFactory.GetValueSetGuid(),
                    new BodyParameter<ClientTermValueSetApiModel>(modelCatalog) { Required = true, Name = "Model" }
                },
                new[]
                {
                    TagsFactory.GetValueSetTag()
                });

            this.RouteDescriber.DescribeRouteWithParams(
                "ChangeValueSetStatus",
                "Updates the status of an existing value set",
                "Updates the status of an existing value set.  Draft may be changed to active.  Active may be changed to Archived.  Archived may be changed to Active.",
                new[]
                {
                    new HttpResponseMetadata<ValueSetApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    ParameterFactory.GetValueSetGuid(),
                    ParameterFactory.GetPathStatusCode()
                },
                new[]
                {
                    TagsFactory.GetValueSetTag()
                });

            this.RouteDescriber.DescribeRouteWithParams(
                "CopyValueSet",
                "Creates a copy of a ValueSet",
                "Creates a copy of a ValueSet",
                new[]
                {
                    new HttpResponseMetadata<ValueSetApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 404, Message = "Not Found" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    new BodyParameter<ValueSetCopyApiModel>(modelCatalog) { Required = true, Name = "Model" }
                },
                new[]
                {
                    TagsFactory.GetValueSetTag()
                });

            this.RouteDescriber.DescribeRouteWithParams(
                "CompareValueSets",
                "Compares two or more ValueSets",
                "Compares two or more ValueSets",
                new[]
                {
                    new HttpResponseMetadata<ValueSetApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    new BodyParameter<CompareValueSetsQuery>(modelCatalog) { Required = true, Name = "Model" }
                },
                new[]
                {
                    TagsFactory.GetValueSetTag()
                });

            this.RouteDescriber.DescribeRouteWithParams(
                "DeleteValueSet",
                "Deletes a client term ValueSet",
                "Deletes a client term ValueSet.  Request is only valid for client term value sets with a 'Draft' status.",
                new[]
                {
                    new HttpResponseMetadata { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 404, Message = "Not Found" },
                    new HttpResponseMetadata { Code = 400, Message = "Bad Request" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[]
                {
                    ParameterFactory.GetValueSetGuid()
                },
                new[]
                {
                    TagsFactory.GetValueSetTag()
                });
        }
    }
}