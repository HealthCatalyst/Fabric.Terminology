namespace Fabric.Terminology.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Persistence;

    public static partial class Extensions
    {
        internal static T Value<T>(this Operation ro) => (T)ro.Value;

        internal static IReadOnlyCollection<Operation> IsOperationOfType<T>(this IEnumerable<Operation> ops)
        {
            return ops.IsOperationOfType(typeof(T));
        }

        internal static IReadOnlyCollection<Operation> IsOperationOfType(
            this IEnumerable<Operation> ops,
            Type type)
        {
            return ops.Where(op => op.Value.GetType() == type).ToList();
        }
    }
}
