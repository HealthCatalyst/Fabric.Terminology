using System;
using Fabric.Terminology.Domain;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.SqlServer.Caching
{
    internal static class CacheKeys
    {
        public static string ValueSetCodesKey(string valueSetId)
        {
            return $"{typeof(IValueSetCode)}-{valueSetId}";
        }
    }
}