namespace Fabric.Terminology.Domain.Services
{
    internal class PersistenceOperation
    {
        public object Value { get; set; }

        public OperationType OperationType { get; set; } = OperationType.Create;
    }
}
