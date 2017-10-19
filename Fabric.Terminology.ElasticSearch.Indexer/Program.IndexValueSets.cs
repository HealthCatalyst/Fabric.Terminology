namespace Fabric.Terminology.ElasticSearch.Indexer
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.ElasticSearch.Elastic;
    using Fabric.Terminology.ElasticSearch.Models;

    using Microsoft.Extensions.DependencyInjection;

    using Nest;

    public partial class Program
    {
        private static async Task<string> IndexValueSetsByPage()
        {
            var indexer = container.GetService<ITerminologyIndexer>();

            var indexName = CreateNewIndexForAlias(indexer, Constants.ValueSetIndexAlias, GetValueSetIndexDescriptor);

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

            var orphanedIndices = ReassignAlias(indexer, indexName, Constants.ValueSetIndexAlias);

            RemoveOrphanedIndices(indexer, orphanedIndices);

            return "success";

            CreateIndexDescriptor GetValueSetIndexDescriptor(string idxName)
            {
                return new CreateIndexDescriptor(idxName).Mappings(m => m.Map<ValueSetIndexModel>(t => t.AutoMap())
                    .Map<ValueSetCodeIndexModel>(t => t.AutoMap())
                    .Map<ValueSetCodeCountIndexModel>(t => t.AutoMap()));
            }
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
