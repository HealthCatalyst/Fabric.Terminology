namespace Fabric.Terminology.Domain
{
    public enum SortDirection
    {
        Asc,
        Desc
    }

#pragma warning disable CA1717 // Only FlagsAttribute enums should have plural names
    public enum ValueSetStatus
    {
        Draft = 1,
        Active = 0,
        Archived = 2
    }
#pragma warning restore CA1717 // Only FlagsAttribute enums should have plural names
}