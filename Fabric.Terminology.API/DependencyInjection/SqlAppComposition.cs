using Fabric.Terminology.API.Configuration;
using Fabric.Terminology.Domain.DependencyInjection;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Configuration;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Nancy.TinyIoc;

namespace Fabric.Terminology.API.DependencyInjection
{
    public class SqlAppComposition : IContainerComposition<TinyIoCContainer>
    {
        public void Compose(TinyIoCContainer container)
        {
            var settings = container.Resolve<IAppConfiguration>().TerminologySqlSettings;

            // TODO - may be able to remove this
            container.Register<TerminologySqlSettings>(settings);
            container.Register<SharedContextFactory>().AsSingleton();

            // Caching
            container.Register<IMemoryCacheProvider, MemoryCacheProvider>().AsSingleton();            
        }
    }
}