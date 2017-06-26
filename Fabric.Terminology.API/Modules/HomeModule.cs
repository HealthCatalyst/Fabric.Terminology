namespace Fabric.Terminology.API.Modules
{
    using Fabric.Terminology.SqlServer.Configuration;
    using Nancy;

    public sealed class HomeModule : NancyModule
    {
        private readonly TerminologySqlSettings _settings;

        public HomeModule(TerminologySqlSettings settings)
        {
            this._settings = settings;

            Get("/", args => _settings.ConnectionString + " " + _settings.UseInMemory.ToString());
        }
    }
}