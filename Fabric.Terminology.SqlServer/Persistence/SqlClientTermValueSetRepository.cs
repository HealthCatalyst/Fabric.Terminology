namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;

    // TODO remove this class now that has been refactored.
    internal class SqlClientTermValueSetRepository : IClientTermValueSetRepository
    {
        private readonly IClientTermUnitOfWorkRepository repository;


        public SqlClientTermValueSetRepository(IClientTermUnitOfWorkRepository unitOfWorkRepository)
        {
            this.repository = unitOfWorkRepository;
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid)
        {
            return this.repository.GetValueSet(valueSetGuid);
        }

        public Attempt<IValueSet> Add(IValueSet valueSet)
        {
            return this.repository.Add(valueSet);
        }

        public void Delete(IValueSet valueSet)
        {
            this.repository.Delete(valueSet);
        }
    }
}