namespace Fabric.Terminology.API.Modules
{
    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.MetaData;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.SqlServer.Configuration;

    using Nancy.Swagger;
    using Nancy.Swagger.Modules;
    using Nancy.Swagger.Services;
    using Nancy.Swagger.Services.RouteUtils;

    using Swagger.ObjectModel;

    public class ValueSetMetadataModule : SwaggerMetadataModule
    {
        public ValueSetMetadataModule(ISwaggerModelCatalog modelCatalog, ISwaggerTagCatalog tagCatalog, TerminologySqlSettings settings)
            : base(modelCatalog, tagCatalog)
        {
            modelCatalog.AddModels(typeof(ValueSetApiModel), typeof(ValueSetCodeApiModel));

            // /{valueSetId}
            this.RouteDescriber.DescribeRouteWithParams(
                "GetValueSet",
                "Returns a single ValueSet",
                "Gets a ValueSet by it's ValueSetId",
                new[]
                {
                    new HttpResponseMetadata { Code = 200, Message = "OK" },
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
                   new HttpResponseMetadata { Code = 200, Message = "OK" },
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
                    new HttpResponseMetadata { Code = 200, Message = "OK" },
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
                    new HttpResponseMetadata { Code = 200, Message = "OK" },
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
                    new HttpResponseMetadata { Code = 200, Message = "OK" },
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
                    new HttpResponseMetadata { Code = 200, Message = "OK" },
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
        }
    }
}