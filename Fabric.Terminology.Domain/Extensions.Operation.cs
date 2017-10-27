namespace Fabric.Terminology.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;

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
            return ops.Where(op => type.IsInstanceOfType(op.Value)).ToList();
        }
    }
}
