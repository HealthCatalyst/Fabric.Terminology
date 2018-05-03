namespace Fabric.Terminology.TestsBase.Fake
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class AsyncEnumerableCollection<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncEnumerableCollection(Expression expression)
            : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetEnumerator() => new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }
}