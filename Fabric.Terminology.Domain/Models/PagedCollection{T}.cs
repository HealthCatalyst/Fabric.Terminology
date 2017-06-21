using System.Collections.Generic;
using System.Linq;

namespace Fabric.Terminology.Domain.Models
{
    public class PagedCollection<TResultType> 
    {
        public int TotalItems { get; set; } = 0;

        public IPagerSettings PagerSettings { get; set; } = new PagerSettings();

        public int TotalPages { get; set; } = 0;

        public bool IsFirstPage => this.PagerSettings.CurrentPage <= 1;

        public bool IsLastPage => this.PagerSettings.CurrentPage >= this.TotalPages;

        public IReadOnlyCollection<TResultType> Items { get; set; } = new List<TResultType>().AsReadOnly();

        public static PagedCollection<TResultType> Empty()
        {
            return new PagedCollection<TResultType>();
        }
    }
}
