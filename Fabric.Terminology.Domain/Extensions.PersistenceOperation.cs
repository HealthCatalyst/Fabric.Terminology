namespace Fabric.Terminology.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Fabric.Terminology.Domain.Services;

    public static partial class Extensions
    {
        internal static T Value<T>(this PersistenceOperation ro) => (T)ro.Value;

        internal static IReadOnlyCollection<PersistenceOperation> IsOperationOfType<T>(this IEnumerable<PersistenceOperation> ops)
        {
            return ops.IsOperationOfType(typeof(T));
        }

        internal static IReadOnlyCollection<PersistenceOperation> IsOperationOfType(
            this IEnumerable<PersistenceOperation> ops,
            Type type)
        {
            return ops.Where(op => type.IsInstanceOfType(op.Value)).ToList();
        }
    }
}
