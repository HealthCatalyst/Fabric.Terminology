﻿namespace Fabric.Terminology.API.Configuration
{
    using Fabric.Terminology.SqlServer.Configuration;

    public interface IAppConfiguration
    {
        TerminologySqlSettings TerminologySqlSettings { get; set; }

        HostingOptions HostingOptions { get; set; }

        IdentityServerConfidentialClientSettings IdentityServerSettings { get; set; }

        ApplicationInsightsSettings ApplicationInsightsSettings { get; set; }
    }
}