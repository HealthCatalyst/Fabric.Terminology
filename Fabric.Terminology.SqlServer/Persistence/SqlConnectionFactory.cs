using System;
using System.Data.SqlClient;

using Fabric.Terminology.Domain;

namespace Fabric.Terminology.SqlServer.Persistence
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