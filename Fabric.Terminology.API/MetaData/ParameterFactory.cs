namespace Fabric.Terminology.API.MetaData
{
    using Swagger.ObjectModel;

    internal static class ParameterFactory
    {
        public static Parameter GetValueSetIdArray()
        {
            return new Parameter
            {
                Name = "valueSetId",
                In = ParameterIn.Query,
                Required = true,
                CollectionFormat = CollectionFormats.Csv,
                Description = "A CSV string of ValueSetIds",
                Type = "string"
            };
        }

        public static Parameter GetCodeSystemCodesArray()
        {
            return new Parameter
            {
                Name = "$codesystems",
                In = ParameterIn.Query,
                Required = false,
                CollectionFormat = CollectionFormats.Csv,
                Description = "An array of Code System Codes (CodeSystemCD) - used to filter ValueSet Codes",
                Type = "string"
            };
        }

        public static Parameter GetSummary()
        {
            return new Parameter
            {
                Name = "$summary",
                Description = "Indicates ValueSets returned should be partial summaries - e.g. codes listing is not complete and intended to be used for presentation.",
                Required = false,
                Type = "boolean",
                Default = true
            };
        }

        public static Parameter GetPageNumber()
        {
            return new Parameter
            {
                Name = "$skip",
                Description = "Skip 'X' number of pages for the 'current' page",
                In = ParameterIn.Query,
                Required = false,
                Default = 0,
                Type = "integer"
            };
        }

        public static Parameter GetItemsPerPage(int defaultItemsPerPage = 20)
        {
            return new Parameter
            {
                Name = "$top",
                Description = "The number of items to be included in a page",
                In = ParameterIn.Query,
                Required = false,
                Default = defaultItemsPerPage,
                Type = "integer"
            };
        }
    }
}