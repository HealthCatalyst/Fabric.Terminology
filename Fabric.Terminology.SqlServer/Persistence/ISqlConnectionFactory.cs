using System.Data.SqlClient;

namespace Fabric.Terminology.SqlServer.Persistence
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateTerminologyConnection();
    }
}