namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSetCode : IValueSetCodeItem
    {
        string VersionDescription { get; }
        string RevisionDescription { get; }
        string CodeDescription { get; }
        IValueSetCodeSystem CodeSystem { get; }
    }
}