namespace Fabric.Terminology.ElasticSearch.Indexer
{
	using Fabric.Terminology.ElasticSearch.Indexer.Nest;

	using global::Nest;

	using Microsoft.Extensions.DependencyInjection;

	internal partial class BootManager
    {
	    private void RegisterNestServices(IServiceCollection services)
	    {
		    services.AddSingleton<ElasticConnectionFactory>();
		    services.AddTransient<ElasticClient>(factory => factory.GetService<ElasticConnectionFactory>().Create());
	    }
    }
}
