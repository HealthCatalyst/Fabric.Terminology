namespace Fabric.Terminology.API.DependencyInjection
{
    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.Domain.DependencyInjection;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Configuration;
    using Fabric.Terminology.SqlServer.Models;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    using Nancy.TinyIoc;

    public class SqlAppComposition : IContainerComposition<TinyIoCContainer>
    {
        public void Compose(TinyIoCContainer container)
        {
            container.Register<TerminologySqlSettings>((c, s) => c.Resolve<IAppConfiguration>().TerminologySqlSettings);
            container.Register<SharedContextFactory>().AsSingleton();
            container.Register<ClientTermContextFactory>().AsSingleton();
            container.Register<IPagingStrategyFactory, PagingStrategyFactory>().AsSingleton();

            container.Register<EmptySamdBinding>().AsSingleton();
        }
    }
}