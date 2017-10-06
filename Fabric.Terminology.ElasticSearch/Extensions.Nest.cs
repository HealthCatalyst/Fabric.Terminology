namespace Fabric.Terminology.ElasticSearch
{
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
    }
}
