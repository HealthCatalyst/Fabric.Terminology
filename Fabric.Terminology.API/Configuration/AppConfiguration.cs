using Fabric.Terminology.Domain.Configuration;

namespace Fabric.Terminology.API.Configuration
{
    public class AppConfiguration : IAppConfiguration
    {
        public TerminologySettings TerminologySettings { get; set; }
    }
}