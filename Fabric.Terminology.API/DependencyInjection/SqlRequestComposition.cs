namespace Fabric.Terminology.API.DependencyInjection
{
    using Fabric.Terminology.Domain.DependencyInjection;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Persistence;
    using Nancy.TinyIoc;

    public class SqlRequestComposition : IContainerComposition<TinyIoCContainer>
    {
        public void Compose(TinyIoCContainer container)
        {
            container.Register<IValueSetCodeRepository, SqlValueSetCodeRepository>();
            container.Register<IValueSetRepository, SqlValueSetRespository>();
        }
    }
}