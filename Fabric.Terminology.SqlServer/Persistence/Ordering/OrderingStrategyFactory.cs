namespace Fabric.Terminology.SqlServer.Persistence.Ordering
{
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal class OrderingStrategyFactory : IOrderingStrategyFactory
    {
        public IOrderingStrategy<ValueSetDescriptionDto> GetValueSetStrategy()
        {
            return new ValueSetBackingItemOrderingStrategy();
        }

        public IOrderingStrategy<ValueSetCodeDto> GetValueSetCodeStrategy()
        {
            return new ValueSetCodeOrderingStrategy();
        }
    }
}