using System;
using Fabric.Terminology.SqlServer.Configuration;
using JetBrains.Annotations;

namespace Fabric.Terminology.SqlServer.Caching
{
    public interface IMemoryCacheProvider : ICacheProvider
    {
        IMemoryCacheSettings Settings { get; }
        object GetItem(string cacheKey, Func<object> getItem, TimeSpan? timeout, bool isSliding = false);
    }
}