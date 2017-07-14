namespace Fabric.Terminology.Domain
{
    public static partial class Extensions
    {
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return (value == null) || (value.Trim().Length == 0);
        }
    }
}
