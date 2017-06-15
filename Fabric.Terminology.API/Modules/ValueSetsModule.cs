using Nancy;

namespace Fabric.Terminology.API.Modules
{
    public class ValueSetsModule : NancyModule
    {
        public ValueSetsModule()
            : base("/api/valuesets")
        {            
            Get("/", arg => "Gets value sets");
        }
    }
}