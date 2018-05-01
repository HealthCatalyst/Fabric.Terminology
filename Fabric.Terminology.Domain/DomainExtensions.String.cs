namespace Fabric.Terminology.Domain
{
    using NullGuard;

    /// <summary>
    /// Extension methods for <see>
    ///         <cref>System.String</cref>
    ///     </see>
    /// </summary>
    public static partial class DomainExtensions
    {
        public static bool IsNullOrWhiteSpace([AllowNull] this string value)
        {
            return (value == null) || (value.Trim().Length == 0);
        }
    }
}
