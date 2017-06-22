namespace Fabric.Terminology.Domain.Models
{
    /// <summary>
    /// Placeholder for object from API post
    /// </summary>
    public interface IValueSetCodeItem
    {
        string Code { get; }
        string ValueSetId { get; }
        string Name { get; }
        string CodeSystemCode { get; }
    }
}