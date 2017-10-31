namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    public class BatchSql
    {
        public string Sql { get; set; }

        public object[] Parameters { get; set; }
    }
}
