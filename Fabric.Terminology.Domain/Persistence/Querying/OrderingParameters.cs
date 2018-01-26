namespace Fabric.Terminology.Domain.Persistence.Querying
{
    public class OrderingParameters : IOrderingParameters
    {
        public SortDirection Direction { get; set; }

        public string FieldName { get; set; }
    }
}