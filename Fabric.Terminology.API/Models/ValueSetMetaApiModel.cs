namespace Fabric.Terminology.API.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    public class ValueSetMetaApiModel : IValueSetMeta
    {
        public string ClientCode { get; set; }

        public DateTime VersionDate { get; set; }

        public string DefinitionDescription { get; set; }

        public string AuthoringSourceDescription { get; set; }

        public string SourceDescription { get; set; }
    }
}