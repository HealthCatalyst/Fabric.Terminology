namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using CallMeMaybe;

    public interface IUnitOfWorkRepository<TEntity>
        where TEntity : class
    {
        TEntity Single(Expression<Func<TEntity, bool>> filter,
            string includeProperties = "");

        IReadOnlyCollection<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        void Insert(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);
    }
}