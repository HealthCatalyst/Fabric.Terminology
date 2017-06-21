using System;
using System.Linq.Expressions;

namespace Fabric.Terminology.Domain.Persistence
{
    public abstract class PageableRepositoryBase<TDto>
        where TDto : class
    {
        /// <summary>
        /// Gets the default sort expression for repository queries based off the repository DTO.
        /// </summary>
        protected abstract Expression<Func<TDto, string>> SortExpression { get; }

        /// <summary>
        /// Gets a value designating the default sort direction for repository queries.
        /// </summary>
        protected abstract SortDirection Direction { get; }
    }
}