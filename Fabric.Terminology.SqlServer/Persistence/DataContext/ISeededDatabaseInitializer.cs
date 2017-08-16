namespace Fabric.Terminology.SqlServer.Persistence.DataContext
{
    using Microsoft.EntityFrameworkCore;

    public interface ISeededDatabaseInitializer<in TContext>
        where TContext : DbContext
    {
        void Initialize(TContext context);
    }
}