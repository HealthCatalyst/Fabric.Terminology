namespace Fabric.Terminology.Domain.Persistence
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class OperationHelper
    {
        public static Operation CreateOperation(object value, OperationType operationType) =>
            new Operation { Value = value, OperationType = operationType };

        public static IEnumerable<Operation> CreateOperationBatch(IEnumerable<object> values, OperationType operationType)
            => values.Select(v => CreateOperation(v, operationType));

        public static IEnumerable<Operation> AppendOperationBatch(
            this Operation operation,
            IEnumerable<object> values,
            OperationType operationType)
            => new List<Operation>
            {
                operation
            }
            .Union(values.Select(v => CreateOperation(v, operationType)));
    }
}
