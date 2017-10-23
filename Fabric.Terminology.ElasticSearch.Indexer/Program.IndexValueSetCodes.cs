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
        private static async Task<string> IndexValueSetCodesByPage()
        {
            var indexer = container.GetService<ITerminologyIndexer>();

            var indexName = CreateNewIndexForAlias(indexer, Constants.ValueSetCodesIndexAlias, GetValueSetCodesIndexDescriptor);

            var service = container.GetService<IValueSetCodeService>();
            var currentPage = 1;
            var page = await QueryPageAndIndex(indexer, service, indexName, currentPage);
            Console.WriteLine($"Completed indexing page {currentPage} of {page.TotalPages}");

            while (currentPage < page.TotalPages)
            {
                currentPage++;
                page = await QueryPageAndIndex(indexer, service, indexName, currentPage);
                Console.WriteLine($"Completed indexing page {currentPage} of {page.TotalPages}");
            }

            Console.WriteLine($"Completed indexing {page.TotalItems} value set codes.");
            logger.Information($"Completed indexing {page.TotalItems} value set codes.");

            var orphanedIndices = ReassignAlias(indexer, indexName, Constants.ValueSetCodesIndexAlias);

            RemoveOrphanedIndices(indexer, orphanedIndices);

            return "success";

            CreateIndexDescriptor GetValueSetCodesIndexDescriptor(string idxName)
            {
                return new CreateIndexDescriptor(idxName).Mappings(m => m.Map<ValueSetCodeIndexModel>(t => t.AutoMap()));
            }
        }

        private static async Task<PagedCollection<IValueSetCode>> QueryPageAndIndex(
            ITerminologyIndexer indexer,
            IValueSetCodeService service,
            string indexName,
            int pageNumber = 1)
        {
            try
            {
                var page = await GetValueSetCodePage(service, pageNumber);
                var indexCount = 0;
                var mapped = page.Values.Select(Mapper.Map<IValueSetCode, ValueSetCodeIndexModel>).ToArray();
                var total = mapped.Length;
                foreach (var batch in mapped.Batch(1000))
                {
                    indexer.IndexMany(batch, indexName);
                    indexCount += 1000;
                    Console.WriteLine($"Completed indexing 1000 value set codes - {indexCount}/{total}");
                }

                return page;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task<PagedCollection<IValueSetCode>> GetValueSetCodePage(
            IValueSetCodeService service,
            int pageNumber,
            int itemsPerPage = 10000)
        {
            var pagerSettings = new PagerSettings { CurrentPage = pageNumber, ItemsPerPage = itemsPerPage };
            var valueSets = await service.GetValueSetCodesAsync(pagerSettings); // we want all versions
            return valueSets;
        }
    }
}
