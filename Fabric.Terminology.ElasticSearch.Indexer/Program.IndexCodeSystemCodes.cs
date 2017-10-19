namespace Fabric.Terminology.ElasticSearch.Indexer
{
    using System;
    using System.Collections.Generic;
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
        private static async Task<string> IndexCodeSystemCodes()
        {
            var indexer = container.GetService<ITerminologyIndexer>();
            var service = container.GetService<ICodeSystemCodeService>();

            try
            {
                var indexName = CreateNewIndexForAlias(indexer, Constants.CodeSystemCodeIndexAlias, GetCodeSystemCodeIndexDescriptor);
                var pagerSettings = new PagerSettings { CurrentPage = 1, ItemsPerPage = int.MaxValue };
                var codes = await service.GetCodeSystemCodesAsync(pagerSettings, true);

                var models = codes.Values.Select(Mapper.Map<CodeSystemCodeIndexModel>).ToArray();

                var indexCount = 0;
                var total = models.Length;
                foreach (var batch in models.Batch(1000))
                {
                    indexer.IndexMany(batch, indexName);
                    indexCount += 1000;
                    Console.WriteLine($"Completed indexing 1000 codes - {indexCount}/{total}");
                }

                Console.WriteLine($"Completed indexing {models.Length} code system codes");
                logger.Information($"Completed indexing {models.Length} code system codes");

                var orphanedIndices = ReassignAlias(indexer, indexName, Constants.CodeSystemCodeIndexAlias);

                RemoveOrphanedIndices(indexer, orphanedIndices);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return "success";

            CreateIndexDescriptor GetCodeSystemCodeIndexDescriptor(string idxName) =>
                new CreateIndexDescriptor(idxName).Mappings(m => m.Map<CodeSystemCodeIndexModel>(t => t.AutoMap()));
        }
    }
}
