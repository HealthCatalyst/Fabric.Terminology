namespace Fabric.Terminology.SqlServer.Caching
{
    using System;

    using CallMeMaybe;

    public interface ICacheProvider
    {
        void ClearAll();

        void ClearItem(string key);

        Maybe<object> GetItem(string key);

        object GetItem(string key, Func<object> getItem);
    }
}