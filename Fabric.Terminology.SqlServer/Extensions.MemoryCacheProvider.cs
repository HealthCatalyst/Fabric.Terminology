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
            where T : class
        {
            return cache.GetItem(key).OfType<T>();
        }

        public static T GetItem<T>(this IMemoryCacheProvider cache, string key, Func<object> getter)
            where T : class
        {
            return cache.GetItem(key, getter)
                    .OfType<T>()
                    .Else(() => throw new InvalidCastException($"Failed to get an item of type {typeof(T)}"));
        }
    }
}
