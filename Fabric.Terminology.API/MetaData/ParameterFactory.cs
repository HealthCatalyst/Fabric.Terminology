namespace Fabric.Terminology.API.MetaData
{
    using Swagger.ObjectModel;

    internal static class ParameterFactory
    {
        public static Parameter GetValueSetId()
        {
            return new Parameter
            {
                Name = "valueSetId",
                In = ParameterIn.Path,
                Required = true,
                Description = "The Id of the ValueSet - typically ValueSetId",
                Type = "string"
            };
        }

        public static Parameter GetValueSetIdArray()
        {
            return new Parameter
            {
                Name = "valueSetId",
                In = ParameterIn.Query,
                Required = true,
                CollectionFormat = CollectionFormats.Csv,
                Description = "An array of ValueSetIds",
                Type = "string"
            };
        }

        public static Parameter GetCodeSystemCodesArray()
        {
            return new Parameter
            {
                Name = "codesystem",
                In = ParameterIn.Query,
                Required = false,
                CollectionFormat = CollectionFormats.Csv,
                Description = "An array of Code System Codes (CodeSystemCD) - used to filter ValueSet Codes",
                Type = "string"
            };
        }

        public static Parameter GetPageNumber()
        {
            return new Parameter
            {
                Name = "p",
                Description = "The page number for the 'current' page",
                In = ParameterIn.Query,
                Required = false,
                Default = 1,
                Type = "integer"
            };
        }

        public static Parameter GetItemsPerPage(int defaultItemsPerPage = 20)
        {
            return new Parameter
            {
                Name = "count",
                Description = "The number of items to be included in a page",
                In = ParameterIn.Query,
                Required = false,
                Default = defaultItemsPerPage,
                Type = "integer"
            };
        }
    }
}