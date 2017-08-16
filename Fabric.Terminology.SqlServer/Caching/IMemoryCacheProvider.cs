﻿namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.SqlServer.Configuration;

    public interface IMemoryCacheProvider : ICacheProvider
    {
        IMemoryCacheSettings Settings { get; }

        IEnumerable<object> GetItems(params string[] cacheKeys);

        object GetItem(string cacheKey, Func<object> getItem, TimeSpan? timeout, bool isSliding = false);
    }
}