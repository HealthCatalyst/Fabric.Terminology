namespace Fabric.Terminology.API.Constants
{
    using System.Collections.Generic;

    internal static class RequiredApiServices
    {
        public static IEnumerable<(string serviceName, int version)> ServicesToDiscover { get; } = new[]
        {
            ("Identity", 1),
            ("Authorization", 1)
        };
    }
}
