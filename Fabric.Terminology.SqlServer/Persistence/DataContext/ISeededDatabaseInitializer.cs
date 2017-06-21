using Microsoft.EntityFrameworkCore;

namespace Fabric.Terminology.SqlServer.Persistence.DataContext
{
    public interface ISeededDatabaseInitializer<in TContext>
        where TContext : DbContext
    {
        void Initialize(TContext context);
    }
}