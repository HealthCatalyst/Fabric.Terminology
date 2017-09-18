namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSetCode : ICodeSetCode
    {
        string ValueSetUniqueId { get; }

        // TODO REMOVE
        string ValueSetOId { get; }

        // TODO REMOVE
        string ValueSetId { get; }

        // TODO REMOVE
        string ValueSetName { get; }
    }
}