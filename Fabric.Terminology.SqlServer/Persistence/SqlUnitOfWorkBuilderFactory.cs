namespace Fabric.Terminology.SqlServer.Persistence
{
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    internal class SqlUnitOfWorkBuilderFactory
    {
        public IValueSetUpdateUnitOfWorkBuilder ValueSetUpdateBuilder(ClientTermContext context)
        {
            return new ValueSetUpdateUnitOfWorkBuilder(context);
        }
    }
}
