namespace Fabric.Terminology.SqlServer
{
    using System;

    using CallMeMaybe;

    using Fabric.Terminology.SqlServer.Caching;

    /// <summary>
    /// Extension methods for <see cref="IMemoryCacheProvider"/>
    /// </summary>
    public static partial class Extensions
    {
        public static Maybe<T> GetItem<T>(this IMemoryCacheProvider cache, string key)
        {
            return cache.GetItem(key).Select(o => (T)o);
        }

        public static Maybe<T> GetItem<T>(this IMemoryCacheProvider cache, string key, Func<object> getter)
        {
            return cache.GetItem(key, getter).Select(o => (T)o);
        }
    }
}
