namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlCrudRepository<TEntity> : ICrudRepository<TEntity>
        where TEntity : class
    {
        private readonly DbContext context;

        private readonly ILogger logger;

        private readonly DbSet<TEntity> dbSet;

        public SqlCrudRepository(DbContext context, ILogger logger)
        {
            this.context = context;
            this.logger = logger;
            this.dbSet = context.Set<TEntity>();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> filter, string includeProperties = "")
        {
            var query = this.dbSet.Where(filter);

            query = includeProperties
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return query.SingleOrDefault();
        }

        public virtual IReadOnlyCollection<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            var query = this.dbSet.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            try
            {
                return orderBy?.Invoke(query).ToList() ?? query.ToList();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"SqlCrudRepository Get failed. {query.ToSql()}");
                throw;
            }
        }

        public void Insert(TEntity entity)
        {
            this.dbSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            this.EnsureAttached(entity);
            this.context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            this.EnsureAttached(entity);
            this.dbSet.Remove(entity);
        }

        private void EnsureAttached(TEntity entity)
        {
            if (this.context.Entry(entity).State == EntityState.Detached)
            {
                this.dbSet.Attach(entity);
            }
        }
    }
}