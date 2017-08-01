namespace Fabric.Terminology.Domain.Models
{
    public class CodeSystem : ICodeSystem
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }
    }
}