namespace Fabric.Terminology.API.Modules
{
    using System;

    using Fabric.Terminology.API.MetaData;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Configuration;

    using Nancy.Swagger;
    using Nancy.Swagger.Modules;
    using Nancy.Swagger.Services;
    using Nancy.Swagger.Services.RouteUtils;

    using Swagger.ObjectModel;

    public class ValueSetMetadataModule : SwaggerMetadataModule
    {
        public ValueSetMetadataModule(
            ISwaggerModelCatalog modelCatalog,
            ISwaggerTagCatalog tagCatalog,
            TerminologySqlSettings settings)
            : base(modelCatalog, tagCatalog)
        {
            modelCatalog.AddModels(
                typeof(CodeSetCodeApiModel),
                typeof(FindByTermQuery),
                typeof(PagedCollection<ValueSetApiModel>),
                typeof(PagedCollection<ValueSetItemApiModel>),
                typeof(PagerSettings),
                typeof(ValueSetApiModel),
                typeof(ValueSetItemApiModel),
                typeof(ValueSetCodeApiModel),
                typeof(ValueSetCodeCountApiModel),
                typeof(ValueSetCreationApiModel),
                typeof(Guid)
                );

            // /{valueSetGuid}
            this.RouteDescriber.DescribeRouteWithParams(
                "GetValueSet",
                "Returns one or more ValueSet(s) by ValueSetGuid(s)",
                "Gets a ValueSet by it's ValueSetGuid or a collection of ValueSets by CSV of ValueSetGuid(s)",
                new[]
                {
                    new HttpResponseMetadata<ValueSetApiModel> { Code = 200, Message = "OK" },
                    new HttpResponseMetadata { Code = 404, Message = "Not Found" },
                    new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
                },
                new[] { ParameterFactory.GetValueSetGuidArray(), ParameterFactory.GetSummary(), ParameterFactory.GetCodeSystemCodesArray() },
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
                    ParameterFactory.GetCodeSystemCodesArray()
                },
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
                    new BodyParameter<FindByTermQuery>(modelCatalog) { Required = true, Name = "Model" }
                },
                new[] { TagsFactory.GetValueSetSearchTag() });

            //this.RouteDescriber.DescribeRouteWithParams(
            //    "AddValueSet",
            //    "Creates a new value set",
            //    "Creates a new value set",
            //    new[]
            //    {
            //        new HttpResponseMetadata<ValueSetApiModel> { Code = 200, Message = "OK" },
            //        new HttpResponseMetadata { Code = 500, Message = "Internal Server Error" }
            //    },
            //    new[]
            //    {
            //        // ParameterFactory.GetContentType(),
            //        new BodyParameter<ValueSetCreationApiModel>(modelCatalog) { Required = true, Name = "Model" }
            //    },
            //    new[]
            //    {
            //        TagsFactory.GetValueSetTag()
            //    });
        }
    }
}