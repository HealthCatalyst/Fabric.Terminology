namespace Fabric.Terminology.API.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    using NullGuard;

    public class ValueSetMetaApiModel : IValueSetMeta
    {
        public string ClientCode { get; set; } = string.Empty;

        [AllowNull]
        public DateTime VersionDate { get; set; }

        public string DefinitionDescription { get; set; } = string.Empty;

        public string AuthorityDescription { get; set; } = string.Empty;

        public string SourceDescription { get; set; } = string.Empty;

        [AllowNull]
        public DateTime LastModifiedDate { get; set; }
    }
}