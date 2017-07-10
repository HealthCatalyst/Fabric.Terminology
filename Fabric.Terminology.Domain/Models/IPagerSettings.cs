namespace Fabric.Terminology.Domain.Models
{
    public interface IPagerSettings
    {
        int CurrentPage { get; set; }

        int ItemsPerPage { get; set; }
    }
}