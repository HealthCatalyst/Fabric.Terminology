namespace Fabric.Terminology.SqlServer.Caching
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    internal static class CacheKeys
    {
        public static string ValueSetKey(string valueSetUniqueId)
        {
            return $"{typeof(ValueSet)}-{valueSetUniqueId}";
        }

        public static string ValueSetKey(string valueSetUniqueId, IEnumerable<string> codeSystemCodes)
        {
            return $"{ValueSetKey(valueSetUniqueId)}-{string.Join(string.Empty, codeSystemCodes)}";
        }

        public static string ValueSetCodesKey(string valueSetUniqueId)
        {
            return $"{typeof(ValueSetCode)}-{valueSetUniqueId}";
        }
    }
}