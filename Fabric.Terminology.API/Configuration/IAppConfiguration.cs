namespace Fabric.Terminology.API.Configuration
{
    using Fabric.Terminology.SqlServer.Configuration;

    public interface IAppConfiguration
    {
        /// <summary>
        /// Should be the URL of the terminology API without the version.
        /// </summary>
        string BaseTerminologyEndpoint { get; set; }

        string SwaggerRootBasePath { get; set; }

        TerminologySqlSettings TerminologySqlSettings { get; set; }

        HostingOptions HostingOptions { get; set; }
    }
}