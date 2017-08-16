namespace Fabric.Terminology.SqlServer.Persistence
{
    using System.Data.SqlClient;

    public interface ISqlConnectionFactory
    {
        SqlConnection CreateTerminologyConnection();
    }
}