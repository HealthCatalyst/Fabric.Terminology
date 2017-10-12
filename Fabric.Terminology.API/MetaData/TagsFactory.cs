namespace Fabric.Terminology.API.MetaData
{
    using Fabric.Terminology.API.Configuration;

    using Swagger.ObjectModel;

    internal static class TagsFactory
    {
        public static Tag GetVersionTag()
        {
            return new Tag
            {
                Description = $"Shared Terminology API services Version {TerminologyVersion.AssemblyVersion}",
                Name = TerminologyVersion.Route
            };
        }

        public static Tag GetValueSetTag()
        {
            return new Tag
            {
                Description = "Operations related to Shared Terminology Value Sets",
                Name = $"{TerminologyVersion.Route}/valuesets/"
            };
        }

        public static Tag GetValueSetVersionTag()
        {
            return new Tag
            {
                Description = "Operations related to Shared Terminology Value Sets Versions",
                Name = $"{TerminologyVersion.Route}/valuesets/versions/"
            };
        }

        public static Tag GetValueSetSearchTag()
        {
            return new Tag
            {
                Description = "Search operations relating to ValueSets",
                Name = $"{TerminologyVersion.Route}/valuesets/search/"
            };
        }

        public static Tag GetCodeSystemTag()
        {
            return new Tag()
            {
                Description = "Operations related to code systems",
                Name = $"{TerminologyVersion.Route}/codesystems"
            };
        }

        public static Tag GetMultipleCodeSystemTag()
        {
            return new Tag
            {
                Description = "Operations relating to getting multiple code systems",
                Name = $"{TerminologyVersion.Route}/codesystems/multiple/"
            };
        }
    }
}