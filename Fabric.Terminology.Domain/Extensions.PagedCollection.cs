using System;
using System.Collections.Generic;
using System.Text;

namespace Fabric.Terminology.Domain
{
    using Fabric.Terminology.Domain.Models;

    /// <summary>
    /// Extension methods for <see cref="PagedCollection{T}"/>
    /// </summary>
    public static partial class Extensions
    {
        public static bool IsFirstPage<T>(this PagedCollection<T> collection)
        {
            return collection.PagerSettings.CurrentPage >= collection.TotalPages;
        }

        public static bool IsLastPage<T>(this PagedCollection<T> collection)
        {
            return  collection.PagerSettings.CurrentPage >= collection.TotalPages;
        }
    }
}
