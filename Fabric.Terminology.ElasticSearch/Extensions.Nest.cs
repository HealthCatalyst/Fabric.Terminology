namespace Fabric.Terminology.ElasticSearch
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    using Nest;

    public static partial class Extensions
    {
        public static SearchDescriptor<T> FromPagerSettings<T>(
            this SearchDescriptor<T> descriptor,
            IPagerSettings settings)
            where T : class
        {
            var pointer = settings.ItemsPerPage * (settings.CurrentPage - 1);
            return descriptor.From(pointer).Size(settings.ItemsPerPage);
        }

        public static BulkAliasDescriptor RemoveFromIndexes(
            this BulkAliasDescriptor descriptor,
            string alias,
            IEnumerable<string> indexNames)
        {
            foreach (var name in indexNames)
            {
                descriptor.Remove(r => r.Alias(alias).Index(name));
            }

            return descriptor;
        }
    }
}