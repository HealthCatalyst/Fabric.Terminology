using Fabric.Terminology.Domain.Configuration;
using Nancy;

namespace Fabric.Terminology.API.Modules
{
    public class HomeModule : NancyModule
    {
        private TerminologySettings _settings;

        public HomeModule(TerminologySettings settings)
        {
            _settings = settings;

            Get("/", args => _settings.ConnectionString);
        }
        
    }
}