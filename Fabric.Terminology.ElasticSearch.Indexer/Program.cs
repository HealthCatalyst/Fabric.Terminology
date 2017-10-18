namespace Fabric.Terminology.ElasticSearch.Indexer
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.ElasticSearch.Configuration;
    using Fabric.Terminology.ElasticSearch.Elastic;
    using Fabric.Terminology.ElasticSearch.Models;

    using global::Nest;

    using Microsoft.Extensions.DependencyInjection;

    using Serilog;

    public class Program
    {
        private const string IndexName = "valuesets";

        private const string MenuFile = "Menu.txt";

        private static IServiceProvider container;

        private static ILogger logger;

        public static void Main(string[] args)
        {
            // Container
            var serviceCollection = new BootManager().Initialize();
            Mapper.Initialize(cfg => cfg.AddProfile<IndexModelProfile>());
            container = serviceCollection.BuildServiceProvider(true);
            logger = container.GetService<ILogger>();

            var arg = string.Empty;
            while (arg != "exit")
            {
                Console.Clear();
                Console.Write(GetMenu());
                arg = Console.ReadLine();

                switch (arg)
                {
                    case "1":
                        // Index value sets
                        Task.Run(async () => await IndexValueSetsByPage());
                        break;
                    case "2":
                        // Index Code Systems
                        break;
                    case "3":
                        // Index Code System Codes
                        break;
                    case "":
                        arg = "exit";
                        break;
                    default:
                        Console.WriteLine($"{arg} is an invalid selection.  Press any key to continue.");
                        arg = string.Empty;
                        Console.ReadLine();
                    break;
                }
            }
        }

        private static string GetMenu()
        {
            var fileName = $"{Directory.GetCurrentDirectory()}\\{MenuFile}";
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Menu file was not found.");
            }

            return File.ReadAllText(fileName);
        }


        private static async Task IndexValueSetsByPage()
        {
            var indexer = container.GetService<ITerminologyIndexer>();
            var indexName = CreateNewIndexForAlias(indexer, Constants.ValueSetIndexAlias);

            var service = container.GetService<IValueSetService>();
            var currentPage = 1;
            var page = await QueryPageAndIndex(indexer, service, indexName, currentPage);
            Console.WriteLine($"Completed indexing page {currentPage} of {page.TotalPages}");

            while (currentPage < page.TotalPages)
            {
                currentPage++;
                page = await QueryPageAndIndex(indexer, service, indexName, currentPage);
                Console.WriteLine($"Completed indexing page {currentPage} of {page.TotalPages}");
            }

            Console.WriteLine($"Completed indexing {page.TotalItems} value sets.");
            logger.Information($"Completed indexing {page.TotalItems} value sets.");

            Console.WriteLine("-------- ALIASING -------------");
            Console.WriteLine($"Checking if alias {Constants.ValueSetIndexAlias} exists on older indexes");
            var existing = indexer.GetIndexesForAlias(Constants.ValueSetIndexAlias);
            if (existing.Any())
            {
                var joined = string.Join(", ", existing);
                Console.WriteLine($"Alias currently assigned to ${joined}");
            }

            Console.WriteLine($"Assigning alias {Constants.ValueSetIndexAlias} to {indexName}");
            indexer.ReassignAlias(indexName, Constants.ValueSetIndexAlias);

            if (existing.Any())
            {
                foreach (var idx in existing)
                {
                    Console.WriteLine($"Deleting old index {idx}");
                    indexer.DropIndex(idx);
                }
            }
        }

        private static string CreateNewIndexForAlias(ITerminologyIndexer indexer, string alias)
        {
            var indexName = indexer.GetNameForIndexByConvention(alias);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Creating new index {indexName}");

            return indexName;
        }

        private static void CleanIndex(ElasticClient client)
        {
            var request = new IndexExistsRequest(IndexName);
            var result = client.IndexExists(request);
            if (result.Exists)
            {
                client.DeleteIndex(IndexName);
            }

            client.CreateIndex(
                IndexName,
                c => c.Mappings(
                    m => m.Map<ValueSetIndexModel>(t => t.AutoMap())
                        .Map<ValueSetCodeIndexModel>(t => t.AutoMap())
                        .Map<ValueSetCodeCountIndexModel>(t => t.AutoMap())
                ));
        }

        private static async Task<PagedCollection<IValueSet>> QueryPageAndIndex(
            ITerminologyIndexer indexer,
            IValueSetService service,
            string indexName,
            int pageNumber = 1)
        {
            try
            {
                var page = await GetValueSetPage(service, pageNumber);
                var mapped = page.Values.Select(Mapper.Map<IValueSet, ValueSetIndexModel>);

                indexer.IndexMany(mapped, indexName);

                return page;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task<PagedCollection<IValueSet>> GetValueSetPage(
            IValueSetService service,
            int pageNumber,
            int itemsPerPage = 50)
        {
            var pagerSettings = new PagerSettings { CurrentPage = pageNumber, ItemsPerPage = itemsPerPage };
            var valueSets = await service.GetValueSetsAsync(pagerSettings, false); // we want all versions
            return valueSets;
        }
    }
}