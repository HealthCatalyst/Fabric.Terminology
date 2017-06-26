namespace Fabric.Terminology.API.Modules
{
    using Nancy;

    public sealed class CodesModule : NancyModule
    {
        public CodesModule()
            : base("/api/codes")
        {
            this.Get("/", arg => "Gets codes");
        }
    }
}