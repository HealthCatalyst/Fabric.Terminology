namespace Fabric.Terminology.ElasticSearch.Elastic
{
    using System;
    using System.Diagnostics;
    using System.Text;

    using Nest;

    using Serilog;

    // This is to test the proof of concept.  Should be load via configuration.
    public class ElasticConnectionFactory
    {
        private readonly ILogger logger;

        public ElasticConnectionFactory(ILogger logger)
        {
            this.logger = logger;
        }

        public ElasticClient Create(bool enableTrace = false) => new ElasticClient(this.GetConnectionSettings(enableTrace));

        private static Uri CreateUri(int port) => new Uri("http://localhost:" + port);

        private ConnectionSettings GetConnectionSettings(bool enableTrace)
        {
            var settings = new ConnectionSettings(CreateUri(9200));
            if (enableTrace)
            {
                settings.PrettyJson().DisableDirectStreaming()
                    .OnRequestCompleted(
                        details =>
                            {
                                var json = Encoding.UTF8.GetString(details.RequestBodyInBytes);
                                Debug.WriteLine(json);
                                this.logger.Debug(json);
                            });
            }

            return settings;
        }
    }
}