using System.Security.Claims;
using Nancy;
using Nancy.Testing;

namespace Fabric.Terminology.UnitTests.Modules
{
   
    /// <seealso cref="https://github.com/HealthCatalyst/Fabric.Authorization/blob/master/Fabric.Authorization.UnitTests/ModuleTestsBase.cs"/>
    public abstract class ModuleTestsBase<T> where T : NancyModule
    {
        protected Browser CreateBrowser(params Claim[] claims)
        {
            return new Browser(CreateBootstrapper(claims), withDefaults => withDefaults.Accept("application/json"));
        }

        private ConfigurableBootstrapper CreateBootstrapper(params Claim[] claims)
        {
            var configurableBootstrapper = new ConfigurableBootstrapper();
            ConfigureBootstrapper(configurableBootstrapper, claims);
            return configurableBootstrapper;
        }

        protected virtual ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator ConfigureBootstrapper(ConfigurableBootstrapper configurableBootstrapper, params Claim[] claims)
        {
            var configurableBootstrapperConfigurator = new ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator(configurableBootstrapper);
            configurableBootstrapperConfigurator.Module<T>();
            configurableBootstrapperConfigurator.RequestStartup((container, pipeline, context) =>
            {
                context.CurrentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "testauthentication"));
            });
            return configurableBootstrapperConfigurator;
        }
    }
}