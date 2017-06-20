namespace Fabric.Terminology.Domain.Models
{
    public class ValueSetCodeSystem : IValueSetCodeSystem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }
}