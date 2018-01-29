namespace Fabric.Terminology.Domain.Persistence.Querying
{
    public class PagingStrategyFactory : IPagingStrategyFactory
    {
        public IPagingStrategy<TResult> GetPagingStrategy<TResult>(int defaultItemsPerPage)
        {
            return new DefaultPagingStrategy<TResult>(defaultItemsPerPage);
        }
    }
}