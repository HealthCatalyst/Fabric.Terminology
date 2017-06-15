using Nancy;

namespace Fabric.Terminology.API.Modules
{
    public sealed class CodesModule : NancyModule
    {
        public CodesModule()
            : base("/api/codes")
        {
            Get("/", arg => "Gets codes");
        }
    }
}