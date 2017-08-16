namespace Fabric.Terminology.Domain.Models
{
    public class PagerSettings : IPagerSettings
    {
        public int CurrentPage { get; set; } = 1;

        public int ItemsPerPage { get; set; } = 500;
    }
}