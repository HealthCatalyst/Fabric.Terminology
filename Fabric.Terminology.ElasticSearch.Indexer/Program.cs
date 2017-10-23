namespace Fabric.Terminology.ElasticSearch.Indexer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using AutoMapper;

    using Fabric.Terminology.ElasticSearch.Configuration;
    using Fabric.Terminology.ElasticSearch.Elastic;

    using Microsoft.Extensions.DependencyInjection;

    using Nest;

    using Serilog;

    public partial class Program
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
                if (arg == "success")
                {
                    Thread.Sleep(2500);
                }

                Console.Clear();
                Console.Write(GetMenu());
                arg = Console.ReadLine();

                switch (arg)
                {
                    case "1":
                        // Index value sets
                        arg = Task.Run(async () => await IndexValueSetsByPage()).Result;
                        break;
                    case "2":
                        arg = Task.Run(async () => await IndexValueSetCodesByPage()).Result;
                        break;
                    case "3":
                        // Index Code Systems
                        arg = IndexCodeSystems();
                        break;
                    case "4":
                        // Index Code System Codes
                        arg = Task.Run(async () => await IndexCodeSystemCodes()).Result;
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

        private static string CreateNewIndexForAlias(ITerminologyIndexer indexer, string alias, Func<string, CreateIndexDescriptor> getDescriptor)
        {
            var indexName = indexer.GetNameForIndexByConvention(alias);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Creating new index {indexName}");

            indexer.CreateIndex(indexName, getDescriptor(indexName));

            return indexName;
        }

        private static IReadOnlyCollection<string> ReassignAlias(ITerminologyIndexer indexer, string newIndexName, string alias)
        {
            Console.WriteLine("-------- ALIASING -------------");
            Console.WriteLine($"Checking if alias {alias} exists on older indexes");
            var existing = indexer.GetIndexesForAlias(alias);
            if (existing.Any())
            {
                var joined = string.Join(", ", existing);
                Console.WriteLine($"Alias currently assigned to {joined}");
                logger.Information($"{alias} Alias currently assigned to {joined}");
            }

            Console.WriteLine($"Reassigning alias {alias} to {newIndexName}");
            logger.Information($"Reassigning alias {alias} to {newIndexName}");
            indexer.ReassignAlias(newIndexName, alias);

            return existing;
        }

        private static void RemoveOrphanedIndices(ITerminologyIndexer indexer, IReadOnlyCollection<string> indices)
        {
            if (!indices.Any())
            {
                return;
            }

            foreach (var idx in indices)
            {
                Console.WriteLine($"Deleting old index {idx}");
                indexer.DropIndex(idx);
                logger.Information($"Deleted index {idx}");
            }
        }
    }
}