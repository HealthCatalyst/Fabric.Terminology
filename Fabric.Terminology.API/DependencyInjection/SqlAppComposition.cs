namespace Fabric.Terminology.API.DependencyInjection
{
    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.Domain.DependencyInjection;
    using Fabric.Terminology.SqlServer.Configuration;
    using Fabric.Terminology.SqlServer.Models;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    using Nancy.TinyIoc;

    public class SqlAppComposition : IContainerComposition<TinyIoCContainer>
    {
        public void Compose(TinyIoCContainer container)
        {
            container.Register<SharedContextFactory>().AsSingleton();
            container.Register<ClientTermContextFactory>().AsSingleton();
            container.Register<EmptySamdBinding>().AsSingleton();
        }
    }
}