using System.Collections.Generic;

namespace Fabric.Terminology.SqlServer.Caching
{
    internal interface ICachePolicy<TItem>
    {
        void AddToCache(string cacheKey, TItem item);

        TItem GetFromCache(string cacheKey, IDictionary<string, object> options);
    }
}