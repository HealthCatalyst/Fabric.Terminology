namespace Fabric.Terminology.Domain
{
    using JetBrains.Annotations;

    using NullGuard;

    /// <summary>
    /// Extension methods for <see>
    ///         <cref>System.String</cref>
    ///     </see>
    /// </summary>
    public static partial class DomainExtensions
    {
        internal static bool IsNullOrWhiteSpace([AllowNull, CanBeNull] this string value)
            => string.IsNullOrWhiteSpace(value);

        internal static string OrEmptyIfNull([AllowNull, CanBeNull] this string value) =>
            string.IsNullOrWhiteSpace(value) ? string.Empty : value;
    }
}
