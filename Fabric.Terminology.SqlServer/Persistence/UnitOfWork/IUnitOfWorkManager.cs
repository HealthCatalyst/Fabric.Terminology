namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Persistence;

    internal interface IUnitOfWorkManager
    {
        IUnitOfWork CreateUnitOfWork(Operation operation);

        IUnitOfWork CreateUnitOfWork(IEnumerable<Operation> operations);
    }
}