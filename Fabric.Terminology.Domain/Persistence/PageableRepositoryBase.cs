namespace Fabric.Terminology.Domain.Persistence
{
    public abstract class PageableRepositoryBase
    {
        /// <summary>
        /// Gets a value designating the default sort or "order by" field for repository queries.
        /// </summary>
        protected abstract string SortField { get; }

        /// <summary>
        /// Gets a value designating the default sort direction for repository queries.
        /// </summary>
        protected abstract SortDirection Direction { get; }
    }
}