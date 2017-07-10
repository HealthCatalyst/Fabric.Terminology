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
                Name = $"{TerminologyVersion.Route}/valueset"
            };
        }

        public static Tag GetValueSetPagedTag()
        {
            return new Tag
            {
                Description = "Operations relating to paged lists of ValueSets",
                Name = $"{TerminologyVersion.Route}/valueset/paged"
            };
        }

        public static Tag GetValueSetFindTag()
        {
            return new Tag
            {
                Description = "Search operations relating to ValueSets",
                Name = $"{TerminologyVersion.Route}/valueset/find"
            };
        }
    }
}