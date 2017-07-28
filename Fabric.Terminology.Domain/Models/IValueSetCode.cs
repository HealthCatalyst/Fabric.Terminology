namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSetCode : ICodeSetCode
    {
        string ValueSetUniqueId { get; }

        string ValueSetId { get; }
    }
}