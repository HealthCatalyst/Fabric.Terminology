namespace Fabric.Terminology.API.DependencyInjection
{
    using Fabric.Terminology.Domain.DependencyInjection;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.ElasticSearch.Elastic;
    using Fabric.Terminology.ElasticSearch.Services;

    using Nancy.TinyIoc;

    using Nest;

    public class ElasticSearchAppComposition : IContainerComposition<TinyIoCContainer>
    {
        public void Compose(TinyIoCContainer container)
        {
            container.Register<ElasticConnectionFactory>().AsSingleton();
            container.Register<IValueSetIndexSearcher, ValueSetIndexSearcher>().AsSingleton();
            container.Register<ElasticClient>((c, s) => c.Resolve<ElasticConnectionFactory>().Create());
            container.Register<IValueSetService, ElasticValueSetService>().AsSingleton();
            container.Register<IValueSetSummaryService, ElasticValueSetSummaryService>().AsSingleton();
        }
    }
}
