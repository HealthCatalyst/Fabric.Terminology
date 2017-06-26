using System;
using JetBrains.Annotations;

namespace Fabric.Terminology.SqlServer.Caching
{
    public interface ICacheProvider
    {
        void ClearAll();
        void ClearItem(string key);
        [CanBeNull]
        object GetItem(string key);
        object GetItem(string key, Func<object> getItem);
    }
}