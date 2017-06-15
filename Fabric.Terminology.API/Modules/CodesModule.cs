using Nancy;

namespace Fabric.Terminology.API.Modules
{
    public class CodesModule : NancyModule
    {
        public CodesModule()
            : base("/api/codes")
        {
            Get("/", arg => "Gets codes");
        }
    }
}