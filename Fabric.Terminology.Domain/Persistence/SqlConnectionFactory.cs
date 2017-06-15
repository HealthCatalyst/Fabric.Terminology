using System;
using System.Data.SqlClient;

namespace Fabric.Terminology.Domain.Persistence
{
    internal class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connString;

        public SqlConnectionFactory(string connnectionString)
        {
            if (connnectionString.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(connnectionString));

            _connString = connnectionString;
        }

        public SqlConnection CreateTerminologyConnection()
        {
            return new SqlConnection(_connString);
        }
    }
}