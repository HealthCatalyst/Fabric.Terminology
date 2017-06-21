namespace Fabric.Terminology.Domain.Models
{    
    public interface IValueSetItem
    {
        string ValueSetId { get; set; }
        string Name { get; set; }
        bool IsCustom { get; }
    }
}