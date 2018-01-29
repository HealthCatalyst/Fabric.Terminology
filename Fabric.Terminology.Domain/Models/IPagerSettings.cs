namespace Fabric.Terminology.Domain.Models
{
    public interface IPagerSettings
    {
        int CurrentPage { get; set; }

        int ItemsPerPage { get; set; }

        string OrderBy { get; set; }

        SortDirection Direction { get; set; }
    }
}