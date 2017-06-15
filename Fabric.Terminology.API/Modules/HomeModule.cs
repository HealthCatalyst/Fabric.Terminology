using Fabric.Terminology.SqlServer.Configuration;
using Nancy;

namespace Fabric.Terminology.API.Modules
{
    public sealed class HomeModule : NancyModule
    {
        private TerminologySqlSettings _settings;

        public HomeModule(TerminologySqlSettings settings)
        {
            _settings = settings;

            Get("/", args => _settings.ConnectionString + " " + _settings.UseInMemory.ToString());
        }       
    }
}