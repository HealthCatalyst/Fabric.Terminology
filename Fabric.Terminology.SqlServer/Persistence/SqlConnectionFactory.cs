namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Data.SqlClient;

    using Fabric.Terminology.Domain;

    internal class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string connString;

        public SqlConnectionFactory(string connnectionString)
        {
            if (connnectionString.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(connnectionString));

            this.connString = connnectionString;
        }

        public SqlConnection CreateTerminologyConnection()
        {
            return new SqlConnection(this.connString);
        }
    }
}