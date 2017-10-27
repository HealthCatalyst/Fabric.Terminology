namespace Fabric.Terminology.SqlServer.Persistence
{
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    internal class ValueSetUpdateUnitOfWork : IUnitOfWork
    {
        private readonly ClientTermContext context;

        private readonly IList<Operation> operations = new List<Operation>();

        public ValueSetUpdateUnitOfWork(ClientTermContext context)
        {
            this.context = context;
        }

        internal IReadOnlyCollection<Operation> Operations => this.operations.ToList();

        public void Add(Operation op)
        {
            this.operations.Add(op);
        }

        public void Add(IEnumerable<Operation> ops)
        {
            foreach (var operation in ops)
            {
                this.Add(operation);
            }
        }

        public void Commit()
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
            }
            throw new System.NotImplementedException();
        }
    }
}
