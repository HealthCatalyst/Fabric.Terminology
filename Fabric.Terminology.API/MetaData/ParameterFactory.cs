namespace Fabric.Terminology.API.MetaData
{
    using Swagger.ObjectModel;

    internal static class ParameterFactory
    {
        public static Parameter GetValueSetGuid()
        {
            return new Parameter
            {
                Name = "valueSetGuid",
                In = ParameterIn.Path,
                Required = true,
                Description = "The ValueSet Guid",
                Type = "string"
            };
        }

        public static Parameter GetCodeSystemGuid()
        {
            return new Parameter
            {
                Name = "codeSystemGuid",
                In = ParameterIn.Path,
                Required = true,
                Description = "The codeSystemGuid for the Code System",
                Type = "string"
            };
        }

        public static Parameter GetCodeGuid()
        {
            return new Parameter
            {
                Name = "codeGuid",
                In = ParameterIn.Path,
                Required = true,
                Description = "The codeGuid for the code system code",
                Type = "string"
            };
        }

        public static Parameter GetCodeSystemGuidsArray()
        {
            return new Parameter
            {
                Name = "$codesystems",
                In = ParameterIn.Query,
                Required = false,
                CollectionFormat = CollectionFormats.Csv,
                Description = "An array of Code System identifiers (CodeSystemGUID) - used to filter ValueSet Codes",
                Type = "string"
            };
        }

        public static Parameter GetValueSetReferenceId()
        {
            return new Parameter
            {
                Name = "referenceId",
                Description = "The published ValueSet (usually the OID)",
                Required = true,
                In = ParameterIn.Path,
                Type = "string"
            };
        }

        public static Parameter GetSummary()
        {
            return new Parameter
            {
                Name = "$summary",
                Description =
                    "Indicates ValueSets returned should be partial summaries - e.g. codes listing is not complete and intended to be used for presentation.",
                Required = false,
                In = ParameterIn.Query,
                Type = "boolean"
            };
        }

        public static Parameter GetQueryStringStatusCode()
        {
            return new Parameter
            {
                Name = "$status",
                Description = "The ValueSet status code.  Valid values are 'Active', 'Draft', 'Archived'",
                Required = false,
                In = ParameterIn.Query,
                CollectionFormat = CollectionFormats.Csv,
                Type = "string"
            };
        }

        public static Parameter GetPathStatusCode()
        {
            return new Parameter
            {
                Name = "statusCode",
                Description = "The ValueSet status code.  Valid values are 'Active', 'Draft', 'Archived'",
                Required = true,
                In = ParameterIn.Path,
                Type = "string",
                Enum = new[] { "Active", "Draft", "Archived" }
            };
        }

        public static Parameter GetContentType()
        {
            return new Parameter
            {
                Name = "Content-Type",
                Description = "Content-Type Header",
                Required = true,
                Type = "string",
                In = ParameterIn.Header,
                Default = "application/json"
            };
        }

        public static Parameter GetSkip()
        {
            return new Parameter
            {
                Name = "$skip",
                Description = "Skip 'X' number of pages for the 'current' page",
                In = ParameterIn.Query,
                Required = false,
                Type = "integer"
            };
        }

        public static Parameter GetTop(int defaultItemsPerPage = 20)
        {
            return new Parameter
            {
                Name = "$top",
                Description = "The number of items to be included in a page",
                In = ParameterIn.Query,
                Required = false,
                Type = "integer"
            };
        }

        public static Parameter GetOrderBy(string orderBy)
        {
            return new Parameter
            {
                Name = "$orderBy",
                Description = "Optional parameter to set the ordering expression.  Default field is 'Name'.  Default direction is 'asc'.  If direction is omitted 'asc' is assumed.",
                Required = false,
                In = ParameterIn.Query,
                Type = "string",
                Enum = new[]
                {
                    "Name asc",
                    "Name desc",
                    "ValueSetReferenceId asc",
                    "ValueSetReferenceId desc",
                    "SourceDescription asc",
                    "SourceDescription desc",
                    "VersionDate asc",
                    "VersionDate desc",
                    "CodeCount asc",
                    "CodeCount desc"
                }
            };
        }
    }
}