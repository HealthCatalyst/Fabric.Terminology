namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;

    internal partial class SqlClientTermUnitOfWorkRepository
    {

        public Attempt<IValueSet> AddSqlBulkCopy(IValueSet valueSet)
        {
            if (!EnsureIsNew(valueSet))
            {
                var invalid = new InvalidOperationException("Cannot save an existing value set as a new value set.");
                return Attempt<IValueSet>.Failed(invalid);
            }

            var valueSetGuid = valueSet.SetIdsForCustomInsert();

            throw new System.NotImplementedException();
        }
    }
}
