namespace Fabric.Terminology.Domain
{
    /// <summary>
    /// Extension methods for <see cref="System.String"/>
    /// </summary>
    public static partial class Extensions
    {
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return (value == null) || (value.Trim().Length == 0);
        }
    }
}
