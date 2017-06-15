using System.Data.SqlClient;

namespace Fabric.Terminology.Domain.Persistence
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateTerminologyConnection();
    }
}