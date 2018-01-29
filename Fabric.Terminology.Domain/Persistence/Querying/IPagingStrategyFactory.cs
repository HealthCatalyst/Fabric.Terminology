namespace Fabric.Terminology.Domain.Persistence.Querying
{
    public interface IPagingStrategyFactory
    {
        IPagingStrategy<TResult> GetPagingStrategy<TResult>(int defaultItemsPerPage);
    }
}