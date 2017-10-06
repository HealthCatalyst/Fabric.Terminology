namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Diagnostics;
    using System.Text;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.ElasticSearch.Configuration;

    using Nest;

    using Serilog;

    // This is to test the proof of concept.  Should be load via configuration.
    public class ElasticConnectionFactory
    {
        private readonly ILogger logger;

        private readonly ElasticSearchSettings settings;

        public ElasticConnectionFactory(ILogger logger, ElasticSearchSettings settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        public ElasticClient Create(bool enableTrace = false) => new ElasticClient(this.GetConnectionSettings(enableTrace));

        private Uri CreateUri()
        {
            var protocol = this.settings.UseSsl ? "https" : "http";
            var port = !this.settings.Port.IsNullOrWhiteSpace() ? $":{this.settings.Port}" : string.Empty;
            return new Uri($"{protocol}://{this.settings.Hostname}{port}");
        }

        private ConnectionSettings GetConnectionSettings(bool enableTrace)
        {
            var connectionSettings = new ConnectionSettings(this.CreateUri());
            if (enableTrace)
            {
                connectionSettings.PrettyJson().DisableDirectStreaming()
                    .OnRequestCompleted(
                        details =>
                            {
                                var json = Encoding.UTF8.GetString(details.RequestBodyInBytes);
                                Debug.WriteLine(json);
                                this.logger.Debug(json);
                            });
            }

            return connectionSettings;
        }
    }
}