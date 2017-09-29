namespace Fabric.Terminology.ElasticSearch.Indexer
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using AutoMapper;

	using Fabric.Terminology.Domain.Models;
	using Fabric.Terminology.Domain.Services;
	using Fabric.Terminology.ElasticSearch.Indexer.Configuration;
	using Fabric.Terminology.ElasticSearch.Indexer.Models;

	using global::Nest;

	using Microsoft.Extensions.DependencyInjection;

	using Serilog;

	public class Program
    {
	    private const string IndexName = "valuesets";

		private static IServiceProvider container;

	    private static ILogger logger;

        public static void Main(string[] args)
        {
            // Container
            var serviceCollection = new BootManager().Initialize();
	        Mapper.Initialize(cfg => cfg.AddProfile<IndexModelProfile>());
	        container = serviceCollection.BuildServiceProvider(true);
			logger = container.GetService<ILogger>();

	        Task.Run(async () => await IndexValueSetsByPage());
	  
			Console.ReadLine();
        }

	    private static async Task IndexValueSetsByPage()
	    {
		    var client = container.GetService<ElasticClient>();
			CleanIndex(client);

			var service = container.GetService<IValueSetService>();
		    var currentPage = 1;
		    var page = await QueryPageAndIndex(client, service, currentPage);
			Console.WriteLine($"Completed indexing page {currentPage} of {page.TotalPages}");

		    while (currentPage < page.TotalPages)
		    {
			    currentPage++;
			    page = await QueryPageAndIndex(client, service, currentPage);
			    Console.WriteLine($"Completed indexing page {currentPage} of {page.TotalPages}");
			}

			Console.WriteLine($"Completed indexing {page.TotalItems} value sets.");
			logger.Information($"Completed indexing {page.TotalItems} value sets.");
	    }

	    private static void CleanIndex(ElasticClient client)
	    {
		    var request = new IndexExistsRequest(IndexName);
			var result = client.IndexExists(request);
		    if (result.Exists)
		    {
			    client.DeleteIndex(IndexName);
		    }
		    client.CreateIndex(IndexName, c => c
			    .Mappings(m => m
				    .Map<ValueSetIndexModel>(t => t
					    .AutoMap()
						.Properties(p => p.Keyword(s => s.Name(n => n.NameKeyword)))
					    //.Properties(p => p.Keyword(s => s.Name(n => n.ValueSetGuid)))
				    )
			    )
		    );
		}

	    private static async Task<PagedCollection<IValueSet>> QueryPageAndIndex(
		    ElasticClient client,
		    IValueSetService service,
		    int pageNumber = 1)
	    {
		    try
		    {
			    var page = await GetValueSetPage(service, pageNumber);
			    var mapped = page.Values.Select(Mapper.Map<IValueSet, ValueSetIndexModel>);

			    var indexResponse = client.Bulk(s => s.IndexMany(mapped,
				    (bulkDescriptor, record) => bulkDescriptor.Index(IndexName).Document(record)));

			    if (!indexResponse.IsValid)
			    {
				    logger.Error(indexResponse.OriginalException, indexResponse.DebugInformation);
			    }

			    return page;
			}
			catch (Exception e)
		    {
			    Console.WriteLine(e);
			    throw;
		    }
	    }

	    private static async Task<PagedCollection<IValueSet>> GetValueSetPage(IValueSetService service, int pageNumber, int itemsPerPage = 50)
	    {
		    var pagerSettings = new PagerSettings { CurrentPage = pageNumber, ItemsPerPage = itemsPerPage };
		    var valueSets = await service.GetValueSetsAsync(pagerSettings);
		    return valueSets;
	    }
    }
}
