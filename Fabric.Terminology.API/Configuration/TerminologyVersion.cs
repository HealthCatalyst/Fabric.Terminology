namespace Fabric.Terminology.API.Configuration
{
    using System;
    using System.Reflection;

    using Fabric.Terminology.Domain;

    using Nancy.Extensions;

    using Semver;

    internal static class TerminologyVersion
    {
        public static Version Current { get; } = new AssemblyName(typeof(TerminologyVersion).GetAssembly().FullName).Version;

        public static string CurrentComment => "alpha001";

        public static string AssemblyVersion => Current.ToString();

        public static string Route => $"v{Current.Major}";

        public static SemVersion SemanticVersion => new SemVersion(
            Current.Major,
            Current.Minor,
            Current.Build,
            CurrentComment.IsNullOrWhiteSpace() ? null : CurrentComment,
            Current.Revision > 0 ? Current.Revision.ToString() : null);
    }
}