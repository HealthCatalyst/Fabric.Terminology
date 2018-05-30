namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1000 // Do not declare static members on generic types
    public class PagedCollection<T>
    {
        public int TotalItems { get; set; } = 0;

        public PagerSettings PagerSettings { get; set; } = new PagerSettings();

        public int TotalPages { get; set; } = 0;

        public IReadOnlyCollection<T> Values { get; set; } = new List<T>().AsReadOnly();

        public static PagedCollection<T> Empty()
        {
            return new PagedCollection<T>();
        }
    }
#pragma warning restore CA1000 // Do not declare static members on generic types
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
}