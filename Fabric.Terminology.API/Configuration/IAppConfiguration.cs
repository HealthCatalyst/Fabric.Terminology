using Fabric.Terminology.Domain.Configuration;

namespace Fabric.Terminology.API.Configuration
{
    public interface IAppConfiguration
    {
        TerminologySettings TerminologySettings { get; set; }
    }
}