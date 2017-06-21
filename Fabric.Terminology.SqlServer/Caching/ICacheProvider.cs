using System;

namespace Fabric.Terminology.SqlServer.Caching
{
    public interface ICacheProvider
    {
        void ClearAll();
        void ClearItem(string key);
        object GetItem(string key);
        object GetItem(string key, Func<object> getItem);
    }
}