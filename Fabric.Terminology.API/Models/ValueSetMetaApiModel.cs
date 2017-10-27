namespace Fabric.Terminology.API.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    // TODO need to expose some of these properties when on creation.
    public class ValueSetMetaApiModel : IValueSetMeta
    {
        public DateTime VersionDate { get; set; }

        public string DefinitionDescription { get; set; }

        public string AuthoringSourceDescription { get; set; }

        public string SourceDescription { get; set; }
    }
}