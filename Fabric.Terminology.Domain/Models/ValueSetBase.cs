using System;

namespace Fabric.Terminology.Domain.Models
{
    public abstract class ValueSetBase : IValueSetBackingItem
    {
        protected ValueSetBase()
        {
        }

        protected ValueSetBase(IValueSetBackingItem backingItem)
        {
            this.ValueSetGuid = backingItem.ValueSetGuid;
            this.Name = backingItem.Name;
            this.VersionDate = backingItem.VersionDate;
            this.AuthoringSourceDescription = backingItem.AuthoringSourceDescription;
            this.DefinitionDescription = backingItem.DefinitionDescription;
            this.SourceDescription = backingItem.SourceDescription;
            this.ValueSetReferenceId = backingItem.ValueSetReferenceId;
            this.OriginGuid = backingItem.OriginGuid;
            this.ClientCode = backingItem.ClientCode;
            this.StatusCode = backingItem.StatusCode;
            this.IsCustom = backingItem.IsCustom;
            this.IsLatestVersion = backingItem.IsLatestVersion;
        }

        public DateTime VersionDate { get; internal set; }

        public string DefinitionDescription { get; internal set; }

        public string AuthoringSourceDescription { get; internal set; }

        public string SourceDescription { get; internal set; }

        public Guid ValueSetGuid { get; internal set; }

        public string ValueSetReferenceId { get; internal set; }

        public string Name { get; internal set; }

        public Guid OriginGuid { get; internal set; }

        public string ClientCode { get; internal set; }

        public ValueSetStatus StatusCode { get; internal set; } = ValueSetStatus.Active;

        public bool IsCustom { get; internal set; }

        public bool IsLatestVersion { get; internal set; }
    }
}
