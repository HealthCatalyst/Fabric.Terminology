namespace Fabric.Terminology.API.MetaData
{
    using Fabric.Terminology.API.Configuration;

    using Swagger.ObjectModel;

    internal static class TagsFactory
    {
        public static Tag GetValueSetTag()
        {
            return new Tag
            {
                Description = "Operations related to Shared Terminology Value Sets",
                Name = $"{TerminologyVersion.Route}/valuesets/"
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

        public static Tag GetCodeSystemCodeTag()
        {
            return new Tag()
            {
                Description = "Operations related to code systems",
                Name = $"{TerminologyVersion.Route}/codes"
            };
        }
    }
}