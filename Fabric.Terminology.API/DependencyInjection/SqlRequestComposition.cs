﻿namespace Fabric.Terminology.API.DependencyInjection
{
    using Fabric.Terminology.Domain.DependencyInjection;
    using Fabric.Terminology.SqlServer.Persistence;
    using Nancy.TinyIoc;

    public class SqlRequestComposition : IContainerComposition<TinyIoCContainer>
    {
        public void Compose(TinyIoCContainer container)
        {
            container.Register<IValueSetCodeRepository, SqlValueSetCodeRepository>();
            container.Register<IValueSetCodeCountRepository, SqlValueSetCodeCountRepository>();
            container.Register<IValueSetBackingItemRepository, SqlValueSetBackingItemRepository>();
            container.Register<IClientTermValueSetRepository, SqlClientTermValueSetRepository>();

            container.Register<ICodeSystemRepository, SqlCodeSystemRepository>();
            container.Register<ICodeSystemCodeRepository, SqlCodeSystemCodeRepository>();
        }
    }
}