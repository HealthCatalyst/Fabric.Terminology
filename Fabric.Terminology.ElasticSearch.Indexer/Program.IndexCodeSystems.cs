namespace Fabric.Terminology.ElasticSearch.Indexer
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.ElasticSearch.Elastic;
    using Fabric.Terminology.ElasticSearch.Models;

    using Microsoft.Extensions.DependencyInjection;

    using Nest;

    public partial class Program
    {
        private static string IndexCodeSystems()
        {
            var indexer = container.GetService<ITerminologyIndexer>();
            var service = container.GetService<ICodeSystemService>();

            try
            {
                var indexName = CreateNewIndexForAlias(indexer, Constants.CodeSystemIndexAlias, GetCodeSystemIndexDescriptor);
                var codeSystems = service.GetAll();
                var models = codeSystems.Select(Mapper.Map<CodeSystemIndexModel>).ToArray();

                indexer.IndexMany(models, indexName);
                Console.WriteLine($"Completed indexing {models.Length} code systems");
                logger.Information($"Completed indexing {models.Length} code systems");

                var orphanedIndices = ReassignAlias(indexer, indexName, Constants.CodeSystemIndexAlias);

                RemoveOrphanedIndices(indexer, orphanedIndices);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return "success";

            CreateIndexDescriptor GetCodeSystemIndexDescriptor(string idxName) =>
                new CreateIndexDescriptor(idxName).Mappings(m => m.Map<CodeSystemIndexModel>(t => t.AutoMap()));
        }
    }
}
