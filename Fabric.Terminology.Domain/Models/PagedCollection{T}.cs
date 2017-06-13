using System.Collections.Generic;
using System.Linq;

namespace Fabric.Terminology.Domain.Models
{
    public class PagedCollection<TResultType> : PagedCollection
    {
        public IEnumerable<TResultType> Items { get; set; }

        public static PagedCollection<TResultType> Empty()
        {
            return new PagedCollection<TResultType>
            {
                CurrentPage = 1,
                Items = Enumerable.Empty<TResultType>(),
                ItemsPerPage = DefaultItemsPerPage,
                TotalItems = 0,
                TotalPages = 0
            };
        }
    }
}
