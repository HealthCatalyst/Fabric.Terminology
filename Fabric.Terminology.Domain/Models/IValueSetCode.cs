namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSetCode : ICodeSetCode
    {
        string ValueSetUniqueId { get; }

        string ValueSetOId { get; }

        string ValueSetId { get; }

        string ValueSetName { get; }
    }
}