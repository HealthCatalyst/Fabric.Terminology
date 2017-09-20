namespace Fabric.Terminology.Domain.Persistence
{
    public class PagingStrategyFactory : IPagingStrategyFactory
    {
        public IPagingStrategy<TSrc, TResult> GetPagingStrategy<TSrc, TResult>(int defaultItemsPerPage)
            where TSrc : class, new()
        {
            return new DefaultPagingStrategy<TSrc, TResult>(defaultItemsPerPage);
        }
    }
}