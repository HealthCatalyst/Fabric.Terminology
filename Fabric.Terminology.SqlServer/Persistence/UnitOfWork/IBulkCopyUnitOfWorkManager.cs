namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using System.Collections.Generic;

    internal interface IBulkCopyUnitOfWorkManager : IUnitOfWorkManager
    {
        IUnitOfWork CreateBulkCopyUnitOfWork<T>(IReadOnlyCollection<T> entities)
            where T : class;
    }
}
