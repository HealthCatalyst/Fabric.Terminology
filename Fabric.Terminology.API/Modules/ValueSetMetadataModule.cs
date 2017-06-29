namespace Fabric.Terminology.API.Modules
{
    using System.Collections.Generic;

    using Fabric.Terminology.API.MetaData;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Configuration;

    using Nancy.Swagger;
    using Nancy.Swagger.Modules;
    using Nancy.Swagger.Services;
    using Nancy.Swagger.Services.RouteUtils;

    public class ValueSetMetadataModule : SwaggerMetadataModule
    {
        public ValueSetMetadataModule(ISwaggerModelCatalog modelCatalog, ISwaggerTagCatalog tagCatalog, TerminologySqlSettings settings)
            : base(modelCatalog, tagCatalog)
        {
            modelCatalog.AddModels(typeof(ValueSetApiModel), typeof(ValueSetCodeApiModel), typeof(FindByTermQuery), typeof(PagerSettings), typeof(PagedCollection<ValueSetApiModel>));

            // /{valueSetId}
            this.RouteDescriber.DescribeRouteWithParams(
                "GetValueSet",
                "Returns a single ValueSet",
                "Gets a ValueSet by it's ValueSetId",
                new[]
                {
                    new HttpResponseMetadata<ValueSetApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 400, Message = "Bad Request" }
                },
                new[]
                {
                    ParameterFactory.GetValueSetId(),
                    ParameterFactory.GetCodeSystemCodesArray()
                },
                new[]
                {
                    TagsFactory.GetValueSetTag()
                });

            // /summary/{valueSetId}
            this.RouteDescriber.DescribeRouteWithParams(
                "GetSummary",
                "Returns a single ValueSet Summary",
                "Gets a ValueSet Summary by it's ValueSetId",
                new[]
                {
                   new HttpResponseMetadata<ValueSetApiModel> { Code = 200, Message = "OK" },
                   new HttpResponseMetadata { Code = 400, Message = "Bad Request" }
                },
                new[]
                {
                    ParameterFactory.GetValueSetId(),
                    ParameterFactory.GetCodeSystemCodesArray()
                },
                new[]
                {
                    TagsFactory.GetValueSetTag()
                });

            // /valuesets/?valuesetid=....
            this.RouteDescriber.DescribeRouteWithParams(
                "GetValueSets",
                "Returns multiple ValueSets",
                "Gets a collection of ValueSets by an array of ValueSetId(s)",
                new[]
                {
                    new HttpResponseMetadata<IEnumerable<ValueSetApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 400, Message = "Bad Request" }
                },
                new[]
                {
                    ParameterFactory.GetValueSetIdArray(),
                    ParameterFactory.GetCodeSystemCodesArray()
                },
                new[]
                {
                    TagsFactory.GetValueSetTag()
                });

            // /summaries/?valuesetid=....
            this.RouteDescriber.DescribeRouteWithParams(
                "GetSummaries",
                "Returns multiple ValueSet Summaries",
                "Gets a collection of ValueSet Summaries by an array of ValueSetId(s)",
                new[]
                {
                    new HttpResponseMetadata<IEnumerable<ValueSetApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 400, Message = "Bad Request" }
                },
                new[]
                {
                    ParameterFactory.GetValueSetIdArray(),
                    ParameterFactory.GetCodeSystemCodesArray()
                },
                new[]
                {
                    TagsFactory.GetValueSetTag()
                });

            this.RouteDescriber.DescribeRouteWithParams(
                "GetPaged",
                "Returns a paged list of ValueSets",
                "Gets a paged collection of ValueSets",
                new[]
                {
                    new HttpResponseMetadata<PagedCollection<ValueSetApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 400, Message = "Bad Request" }
                },
                new[]
                {
                    ParameterFactory.GetPageNumber(),
                    ParameterFactory.GetItemsPerPage(settings.DefaultItemsPerPage),
                    ParameterFactory.GetCodeSystemCodesArray()
                },
                new[]
                {
                    TagsFactory.GetValueSetPagedTag()
                });

            this.RouteDescriber.DescribeRouteWithParams(
                "GetPagedSummaries",
                "Returns a paged list of ValueSet summaries",
                "Gets a paged collection of ValueSet summaries",
                new[]
                {
                    new HttpResponseMetadata<PagedCollection<ValueSetApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 400, Message = "Bad Request" }
                },
                new[]
                {
                    ParameterFactory.GetPageNumber(),
                    ParameterFactory.GetItemsPerPage(settings.DefaultItemsPerPage),
                    ParameterFactory.GetCodeSystemCodesArray()
                },
                new[]
                {
                    TagsFactory.GetValueSetPagedTag()
                });

            this.RouteDescriber.DescribeRouteWithParams(
                "Find",
                "Search by 'Name' of ValueSet operation",
                "Gets a paged collection of ValueSet's matching the 'Name' filter",
                new[]
                {
                    new HttpResponseMetadata<PagedCollection<ValueSetApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 400, Message = "Bad Request" }
                },
                new[]
                {
                    new BodyParameter<FindByTermQuery>(modelCatalog) { Required = false }
                },
                new[]
                {
                    TagsFactory.GetValueSetFindTag()
                });

            this.RouteDescriber.DescribeRouteWithParams(
                "FindSummaries",
                "Search by 'Name' of ValueSet operation",
                "Gets a paged collection of ValueSet summaries matching the 'Name' filter",
                new[]
                {
                    new HttpResponseMetadata<PagedCollection<ValueSetApiModel>> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 400, Message = "Bad Request" }
                },
                new[]
                {
                    new BodyParameter<FindByTermQuery>(modelCatalog) { Required = false }
                },
                new[]
                {
                    TagsFactory.GetValueSetFindTag()
                });
        }
    }
}