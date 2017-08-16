namespace Fabric.Terminology.API
{
    using Fabric.Terminology.API.Configuration;

    using Microsoft.AspNetCore.Hosting;

    public static partial class Extensions
    {
        internal static IWebHostBuilder UseIisIntegrationIfConfigured(this IWebHostBuilder builder, IAppConfiguration appConfiguration)
        {
            if (appConfiguration.HostingOptions != null && appConfiguration.HostingOptions.UseIis)
            {
                builder.UseIISIntegration();
            }

            return builder;
        }
    }
}
