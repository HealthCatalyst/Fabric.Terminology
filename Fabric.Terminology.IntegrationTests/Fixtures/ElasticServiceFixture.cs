namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.ElasticSearch.Configuration;
    using Fabric.Terminology.ElasticSearch.Elastic;
    using Fabric.Terminology.ElasticSearch.Services;
    using Fabric.Terminology.TestsBase.Fixtures;

    using Nest;

    public class ElasticServiceFixture : TestFixtureBase
    {
        public ElasticServiceFixture()
        {
            var settings =
                new ElasticSearchSettings { Enabled = true, Hostname = "localhost", Port = "9200", UseSsl = false };

            var factory = new ElasticConnectionFactory(this.Logger, settings);
            this.ElasticClient = factory.Create();

            var searcher = new ValueSetIndexSearcher(this.Logger, this.ElasticClient, new PagingStrategyFactory());

            var codeSystemSearcher = new CodeSystemSearcher(this.Logger, this.ElasticClient);

            this.ValueSetService = new ElasticValueSetService(searcher);
            this.ValueSetSummaryService = new ElasticValueSetSummaryService(searcher);
            this.CodeSystemService = new ElasticCodeSystemService(codeSystemSearcher);
        }

        public ElasticClient ElasticClient { get; }

        public IValueSetService ValueSetService { get; }

        public IValueSetSummaryService ValueSetSummaryService { get; }

        public ICodeSystemService CodeSystemService { get; }

        protected override bool EnableLogging => true;
    }
}