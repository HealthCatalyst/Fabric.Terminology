namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSetCodeItem
    {
        string Code { get; }
        string ValueSetId { get; }
        string Name { get; }
    }
}