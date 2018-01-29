namespace Fabric.Terminology.Domain.Persistence.Querying
{
    public interface IOrderingParameters
    {
        SortDirection Direction { get; set; }
        string FieldName { get; set; }
    }
}