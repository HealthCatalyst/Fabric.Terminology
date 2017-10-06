namespace Fabric.Terminology.API.DependencyInjection
{
    using System;

    using Fabric.Terminology.Domain.DependencyInjection;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Services;

    using Nancy.TinyIoc;

    public class SqlServicesComposition : IContainerComposition<TinyIoCContainer>
    {
        public void Compose(TinyIoCContainer container)
        {
            container.Register<IValueSetCodeRepository, SqlValueSetCodeRepository>();
            container.Register<IValueSetCodeCountRepository, SqlValueSetCodeCountRepository>();
            container.Register<IValueSetBackingItemRepository, SqlValueSetBackingItemRepository>();
            container.Register<IClientTermValueSetRepository, SqlClientTermValueSetRepository>();
            container.Register<SharedContext>((c, p) => c.Resolve<SharedContextFactory>().Create());
            container.Register<Lazy<ClientTermContext>>((c, p) => c.Resolve<ClientTermContextFactory>().CreateLazy());
            container.Register<IValueSetService, SqlValueSetService>().AsSingleton();
            container.Register<IValueSetSummaryService, SqlValueSetSummaryService>().AsSingleton();
        }
    }
}