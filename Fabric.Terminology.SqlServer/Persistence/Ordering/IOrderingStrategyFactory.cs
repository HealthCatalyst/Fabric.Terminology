namespace Fabric.Terminology.SqlServer.Persistence.Ordering
{
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal interface IOrderingStrategyFactory
    {
        IOrderingStrategy<ValueSetDescriptionDto> GetValueSetStrategy();

        IOrderingStrategy<ValueSetCodeDto> GetValueSetCodeStrategy();
    }
}