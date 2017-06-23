namespace Fabric.Terminology.SqlServer
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Caching;

    using JetBrains.Annotations;

    /// <summary>
    /// Extension methods for <see cref="IMemoryCacheProvider"/>
    /// </summary>
    public static partial class Extensions
    {
        [CanBeNull]
        internal static IValueSet GetCachedValueSetWithAllCodes(this IMemoryCacheProvider cache, string valueSetId)
        {
            var cacheKey = CacheKeys.ValueSetKey(valueSetId);
            var fnd = (IValueSet)cache.GetItem(cacheKey);
            if (fnd != null && fnd.AllCodesLoaded)
            {
                return fnd;
            }

            return null;
        }
    }
}
