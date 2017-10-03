namespace Fabric.Terminology.ElasticSearch.Indexer
{
    using Fabric.Terminology.ElasticSearch.Elastic;

    using global::Nest;

    using Microsoft.Extensions.DependencyInjection;

    internal partial class BootManager
    {
        private void RegisterNestServices(IServiceCollection services)
        {
            services.AddTransient<ElasticConnectionFactory>();
            services.AddSingleton<ElasticClient>(factory => factory.GetService<ElasticConnectionFactory>().Create());
        }
    }
}