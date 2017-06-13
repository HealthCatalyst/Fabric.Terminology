namespace Fabric.Terminology.Domain.Models
{
    public interface IPagerSettings
    {
        long CurrentPage { get; set; }
        long ItemsPerPage { get; set; }        
    }
}