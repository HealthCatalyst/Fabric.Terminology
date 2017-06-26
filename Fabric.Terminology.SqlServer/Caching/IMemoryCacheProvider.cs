using System;
using Fabric.Terminology.SqlServer.Configuration;
using JetBrains.Annotations;

namespace Fabric.Terminology.SqlServer.Caching
{
    using System.Collections.Generic;

    public interface IMemoryCacheProvider : ICacheProvider
    {
        IMemoryCacheSettings Settings { get; }

        IEnumerable<object> GetItems(params string[] cacheKeys);

        object GetItem(string cacheKey, Func<object> getItem, TimeSpan? timeout, bool isSliding = false);
    }
}