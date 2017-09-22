namespace Fabric.Terminology.Domain.Persistence
{
    public interface IPagingStrategyFactory
    {
        IPagingStrategy<TResult> GetPagingStrategy<TResult>(int defaultItemsPerPage);
    }
}