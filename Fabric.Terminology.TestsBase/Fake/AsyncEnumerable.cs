namespace Fabric.Terminology.TestsBase.Fake
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetEnumerator() => new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }
}