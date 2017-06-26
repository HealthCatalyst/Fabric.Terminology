using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.SqlServer.Caching
{
    internal static class CacheKeys
    {
        public static string ValueSetKey(string valueSetId, params string[] codeSystemCodes)
        {
            return $"{typeof(ValueSet)}-{valueSetId}-{string.Join(string.Empty, codeSystemCodes)}";
        }

        public static string ValueSetCodesKey(string valueSetId)
        {
            return $"{typeof(ValueSetCode)}-{valueSetId}";
        }
    }
}