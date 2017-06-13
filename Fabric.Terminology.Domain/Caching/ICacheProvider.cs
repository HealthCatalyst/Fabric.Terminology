using System;
using System.Runtime.InteropServices.ComTypes;

namespace Fabric.Terminology.Domain.Caching
{
    public interface ICacheProvider
    {
        void ClearAll();
        void ClearItem(string key);
        object GetItem(string key);
        object GetItem(string key, Func<object> getItem);
    }
}