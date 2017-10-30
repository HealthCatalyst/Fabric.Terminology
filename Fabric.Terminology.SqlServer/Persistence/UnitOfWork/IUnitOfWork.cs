namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Persistence;

    internal interface IUnitOfWork
    {
        void Commit(Queue<Operation> operations);
    }
}