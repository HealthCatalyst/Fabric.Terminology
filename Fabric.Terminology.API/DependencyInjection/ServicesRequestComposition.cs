using System;
using Fabric.Terminology.Domain.DependencyInjection;
using Fabric.Terminology.Domain.Services;
using Fabric.Terminology.SqlServer.Persistence;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Nancy.TinyIoc;

namespace Fabric.Terminology.API.DependencyInjection
{
    public class ServicesRequestComposition : IContainerComposition<TinyIoCContainer>
    {
        public void Compose(TinyIoCContainer container)
        {
            container.Register<SharedContext>((c, p) => c.Resolve<SharedContextFactory>().Create());
            container.Register<IValueSetCodeService, ValueSetCodeService>().AsSingleton();
            container.Register<IValueSetService, ValueSetService>().AsSingleton();
        }
    }
}