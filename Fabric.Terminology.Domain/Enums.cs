namespace Fabric.Terminology.Domain
{
    // TODO remove this and replace with something that does not need to be mapped.
    /// We don't want to have a hard dependency on EF or some other ORM in this library ...
    public enum SortDirection
    {
        Ascending,
        Descending
    }
}