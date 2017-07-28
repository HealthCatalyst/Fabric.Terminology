namespace Fabric.Terminology.API.DependencyInjection
{
    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.Domain.DependencyInjection;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Configuration;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    using Nancy.TinyIoc;

    public class SqlAppComposition : IContainerComposition<TinyIoCContainer>
    {
        public void Compose(TinyIoCContainer container)
        {
            container.Register<TerminologySqlSettings>((c, s) => c.Resolve<IAppConfiguration>().TerminologySqlSettings);
            container.Register<SharedContextFactory>().AsSingleton();
            container.Register<ClientTermContextFactory>().AsSingleton();

            container.Register<IPagingStrategy<ValueSetCodeDto, IValueSetCode>>(
                (c, s) => new DefaultPagingStrategy<ValueSetCodeDto, IValueSetCode>(
                    c.Resolve<TerminologySqlSettings>().DefaultItemsPerPage));

            container.Register<IPagingStrategy<ValueSetDescriptionDto, IValueSet>>(
                (c, s) => new DefaultPagingStrategy<ValueSetDescriptionDto, IValueSet>(
                    c.Resolve<TerminologySqlSettings>().DefaultItemsPerPage));
        }
    }
}