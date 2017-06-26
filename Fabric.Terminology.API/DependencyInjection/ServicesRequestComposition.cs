namespace Fabric.Terminology.API.DependencyInjection
{
    using Fabric.Terminology.Domain.DependencyInjection;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Nancy.TinyIoc;

    public class ServicesRequestComposition : IContainerComposition<TinyIoCContainer>
    {
        public void Compose(TinyIoCContainer container)
        {
            container.Register<SharedContext>((c, p) => c.Resolve<SharedContextFactory>().Create());
            container.Register<IValueSetService, ValueSetService>().AsSingleton();
        }
    }
}