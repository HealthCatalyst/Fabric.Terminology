namespace Fabric.Terminology.ElasticSearch.Indexer
{
    using Fabric.Terminology.ElasticSearch.Configuration;
    using Fabric.Terminology.ElasticSearch.Elastic;
    using Fabric.Terminology.ElasticSearch.Indexer.Configuration;

    using global::Nest;

    using Microsoft.Extensions.DependencyInjection;

    internal partial class BootManager
    {
        private void RegisterNestServices(IServiceCollection services)
        {
            services.AddSingleton<ElasticSearchSettings>(factory => factory.GetService<IndexerConfiguration>().ElasticSearchSettings);
            services.AddTransient<ElasticConnectionFactory>();
            services.AddSingleton<ElasticClient>(factory => factory.GetService<ElasticConnectionFactory>().Create());
            services.AddSingleton<ITerminologyIndexer, TerminologyIndexer>();
        }
    }
}