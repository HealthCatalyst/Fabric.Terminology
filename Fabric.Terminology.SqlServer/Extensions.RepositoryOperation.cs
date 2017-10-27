namespace Fabric.Terminology.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Fabric.Terminology.SqlServer.Persistence;

    public static partial class Extensions
    {
        internal static T Value<T>(this RepositoryOperation ro) => (T)ro.Value;

        internal static IReadOnlyCollection<RepositoryOperation> IsOperationOfType<T>(this IEnumerable<RepositoryOperation> ops)
        {
            return ops.IsOperationOfType(typeof(T));
        }

        internal static IReadOnlyCollection<RepositoryOperation> IsOperationOfType(
            this IEnumerable<RepositoryOperation> ops,
            Type type)
        {
            return ops.Where(op => type.IsInstanceOfType(op.Value)).ToList();
        }
    }
}
