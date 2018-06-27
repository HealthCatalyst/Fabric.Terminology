namespace Fabric.Terminology.API.Configuration
{
    using System;
    using System.Globalization;
    using System.Reflection;

    using Fabric.Terminology.Domain;

    using Semver;

    internal static class TerminologyVersion
    {
        public const string DiscoveryServiceName = "TerminologyService";

        public static Version Current { get; } = new AssemblyName(typeof(TerminologyVersion).GetTypeInfo().Assembly.FullName).Version;

#pragma warning disable SA1122 // Use string.Empty for empty strings
        public static string CurrentComment => "";
#pragma warning restore SA1122 // Use string.Empty for empty strings

        public static string AssemblyVersion => Current.ToString();

        public static string Route => $"v{Current.Major}.{Current.Minor}";

        public static SemVersion SemanticVersion => new SemVersion(
            Current.Major,
            Current.Minor,
            Current.Build,
            CurrentComment.IsNullOrWhiteSpace() ? null : CurrentComment,
            Current.Revision > 0 ? Current.Revision.ToString(CultureInfo.InvariantCulture) : null);
    }
}