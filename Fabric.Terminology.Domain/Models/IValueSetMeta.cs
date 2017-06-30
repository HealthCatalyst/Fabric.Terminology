namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSetMeta
    {
        string AuthoringSourceDescription { get; }
        string PurposeDescription { get; }
        string SourceDescription { get; }
        string VersionDescription { get; }
    }
}