namespace Fabric.Terminology.Domain.Persistence
{
    using Fabric.Terminology.Domain.Services;

    internal class Operation
    {
        public object Value { get; set; }

        public OperationType OperationType { get; set; } = OperationType.Create;
    }
}
