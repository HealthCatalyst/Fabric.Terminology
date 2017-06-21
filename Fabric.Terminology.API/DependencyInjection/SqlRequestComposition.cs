using Fabric.Terminology.Domain.DependencyInjection;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Persistence;
using Nancy.TinyIoc;

namespace Fabric.Terminology.API.DependencyInjection
{
    public class SqlRequestComposition : IContainerComposition<TinyIoCContainer>
    {
        public void Compose(TinyIoCContainer container)
        {
            //var options = 


            container.Register<IValueSetCodeRepository, SqlValueSetCodeRepository>();
            container.Register<IValueSetRepository, SqlValueSetRespository>();
        }
    }
}