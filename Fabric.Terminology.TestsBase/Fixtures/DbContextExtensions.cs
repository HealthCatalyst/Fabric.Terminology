namespace Fabric.Terminology.TestsBase.Fixtures
{
    using System;

    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    internal static class DbContextExtensions
    {
        public static Lazy<ClientTermContext> AsLazy(this ClientTermContext context)
        {
            return new Lazy<ClientTermContext>(() => context);
        }
    }
}