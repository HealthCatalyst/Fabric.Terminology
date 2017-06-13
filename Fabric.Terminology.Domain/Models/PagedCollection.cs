namespace Fabric.Terminology.Domain.Models
{
    public class PagedCollection : IPagerSettings
    {
        /// TODO - Default for the PRB application.  May be useful to have this configurable.
        protected const long DefaultItemsPerPage = 500;


        public long TotalItems { get; set; } = 0;

        public long CurrentPage { get; set; } = 1;

        public long ItemsPerPage { get; set; } = DefaultItemsPerPage;

        public long TotalPages { get; set; } = 0;
        
        public bool IsFirstPage => this.CurrentPage <= 1;

        public bool IsLastPage => this.CurrentPage >= this.TotalPages;
    }
}