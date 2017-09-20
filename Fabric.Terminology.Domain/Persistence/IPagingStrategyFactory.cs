namespace Fabric.Terminology.Domain.Persistence
{
    public interface IPagingStrategyFactory
    {
        IPagingStrategy<TSrc, TResult> GetPagingStrategy<TSrc, TResult>(int defaultItemsPerPage)
            where TSrc : class, new();
    }
}