namespace Fabric.Terminology.Domain
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Querying;

    public static partial class Extensions
    {
        public static IOrderingParameters AsOrderingParameters(this IPagerSettings pagerSettings) =>
            new OrderingParameters { FieldName = pagerSettings.OrderBy, Direction = pagerSettings.Direction };
    }
}
