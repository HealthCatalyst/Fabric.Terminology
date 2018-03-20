namespace Fabric.Terminology.SqlServer.Persistence.Ordering
{
    using System.Linq;

    using Fabric.Terminology.Domain.Persistence.Querying;

    public interface IOrderingStrategy<TResult>
    {
        IQueryable<TResult> SetOrdering(IQueryable<TResult> query, IOrderingParameters parameters);
    }
}