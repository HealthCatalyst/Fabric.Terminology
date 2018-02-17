namespace Fabric.Terminology.Domain.Models
{
    using System;

    internal class ValueSetBackingItem : IValueSetBackingItem
    {
        public Guid ValueSetGuid { get; internal set; }

        public string ValueSetReferenceId { get; internal set; }

        public DateTime VersionDate { get; internal set; }

        public string DefinitionDescription { get; internal set; }

        public string AuthorityDescription { get; internal set; }

        public string SourceDescription { get; internal set; }

        public string Name { get; internal set; }

        public Guid OriginGuid { get; internal set; }

        public string ClientCode { get; internal set; }

        public ValueSetStatus StatusCode { get; internal set; }

        public bool IsCustom { get; internal set; }

        public bool IsLatestVersion { get; internal set; }
    }
}
