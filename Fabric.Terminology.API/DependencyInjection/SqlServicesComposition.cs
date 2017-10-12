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
            container.Register<IValueSetService, SqlValueSetService>().AsSingleton();
            container.Register<IValueSetSummaryService, SqlValueSetSummaryService>().AsSingleton();

        }
    }
}