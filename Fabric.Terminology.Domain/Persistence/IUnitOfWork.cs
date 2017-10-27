namespace Fabric.Terminology.Domain.Persistence
{
    using System.Collections.Generic;

    internal interface IUnitOfWork
    {
        void Add(Operation op);

        void Add(IEnumerable<Operation> ops);

        void Commit();
    }
}