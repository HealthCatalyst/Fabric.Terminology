namespace Fabric.Terminology.API.Models
{
    using Fabric.Terminology.Domain.Models;

    public class ValueSetMetaApiModel : IValueSetMeta
    {
        public string AuthoringSourceDescription { get; set; }

        public string PurposeDescription { get; set; }

        public string SourceDescription { get; set; }

        public string VersionDescription { get; set; }
    }
}