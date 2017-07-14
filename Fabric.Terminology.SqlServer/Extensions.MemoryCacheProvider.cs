namespace Fabric.Terminology.SqlServer
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Caching;

    using JetBrains.Annotations;

    /// <summary>
    /// Extension methods for <see cref="IMemoryCacheProvider"/>
    /// </summary>
    public static partial class Extensions
    {
        [CanBeNull]
        internal static IValueSet GetCachedValueSetWithAllCodes(this IMemoryCacheProvider cache, string valueSetId, IReadOnlyCollection<string> codeSystemCodes)
        {
            var cacheKey = CacheKeys.ValueSetKey(valueSetId, codeSystemCodes);
            var fnd = (IValueSet)cache.GetItem(cacheKey);
            if (fnd != null && fnd.AllCodesLoaded)
            {
                return fnd;
            }

            return null;
        }
    }
}
