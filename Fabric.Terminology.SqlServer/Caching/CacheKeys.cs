namespace Fabric.Terminology.SqlServer.Caching
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    internal static class CacheKeys
    {
        public static string ValueSetKey(string valueSetId)
        {
            return $"{typeof(ValueSet)}-{valueSetId}";
        }

        public static string ValueSetKey(string valueSetId, IEnumerable<string> codeSystemCodes)
        {
            return $"{ValueSetKey(valueSetId)}-{string.Join(string.Empty, codeSystemCodes)}";
        }

        public static string ValueSetCodesKey(string valueSetId)
        {
            return $"{typeof(ValueSetCode)}-{valueSetId}";
        }
    }
}