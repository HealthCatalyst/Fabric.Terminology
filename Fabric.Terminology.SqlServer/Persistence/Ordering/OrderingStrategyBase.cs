namespace Fabric.Terminology.SqlServer.Persistence.Ordering
{
    using System;
    using System.Linq;

    using Fabric.Terminology.Domain.Persistence.Querying;

    public abstract class OrderingStrategyBase<TResult> : IOrderingStrategy<TResult>
    {
        public IQueryable<TResult> SetOrdering(IQueryable<TResult> query, IOrderingParameters parameters)
        {
            if (this.IsOrderedQueryable(query))
            {
                throw new ArgumentException("IQueryable<T> is already ordered.", nameof(query));
            }

            return this.PerformOrdering(query, parameters);
        }

        protected abstract IQueryable<TResult> PerformOrdering(IQueryable<TResult> query, IOrderingParameters parameters);

        private bool IsOrderedQueryable(IQueryable<TResult> query) =>
            query.Expression.Type == typeof(IOrderedQueryable<TResult>);
    }
}