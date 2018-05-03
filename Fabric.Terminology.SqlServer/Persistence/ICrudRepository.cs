namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public interface ICrudRepository<TEntity>
        where TEntity : class
    {
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> filter,
            string includeProperties = "");

        IReadOnlyCollection<TEntity> GetEntity(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        void Insert(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);
    }
}