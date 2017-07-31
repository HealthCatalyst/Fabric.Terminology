namespace Fabric.Terminology.API.Configuration
{
    using Fabric.Terminology.Domain.Configuration;
    using Fabric.Terminology.SqlServer.Configuration;

    public interface IAppConfiguration
    {
        TerminologySqlSettings TerminologySqlSettings { get; set; }

        ValueSetSettings ValueSetSettings { get; set; }

        HostingOptions HostingOptions { get; set; }
    }
}