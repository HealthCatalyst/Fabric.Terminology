namespace Fabric.Terminology.Domain
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Visitors;

    public static partial class Extensions
    {
        public static void Accept<T>(this IEnumerable<T> source, IVisitor<T> visitor)
        {
            foreach (var item in source)
            {
                visitor.Visit(item);
            }
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(
            this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return YieldBatchElements(enumerator, batchSize - 1);
                }
            }

            IEnumerable<T> YieldBatchElements(IEnumerator<T> src, int size)
            {
                yield return src.Current;
                for (var i = 0; i < size && src.MoveNext(); i++)
                {
                    yield return src.Current;
                }
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
    }
}
