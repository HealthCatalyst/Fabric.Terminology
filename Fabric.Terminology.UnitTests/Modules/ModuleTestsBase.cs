namespace Fabric.Terminology.UnitTests.Modules
{
    using System.Security.Claims;
    using Fabric.Terminology.TestsBase.Mocks;
    using Nancy;
    using Nancy.Testing;

    /// <summary>
    /// Test base for Nancy module tests
    /// </summary>
    /// <seealso cref="https://github.com/HealthCatalyst/Fabric.Authorization/blob/master/Fabric.Authorization.UnitTests/ModuleTestsBase.cs"/>
    public abstract class ModuleTestsBase<T>
        where T : NancyModule
    {
        protected Browser CreateBrowser(params Claim[] claims)
        {
            return new Browser(this.CreateBootstrapper(claims), withDefaults => withDefaults.Accept("application/json"));
        }

        protected virtual ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator ConfigureBootstrapper(ConfigurableBootstrapper configurableBootstrapper, params Claim[] claims)
        {
            var configurableBootstrapperConfigurator = new ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator(configurableBootstrapper);
            configurableBootstrapperConfigurator.Module<T>();
            configurableBootstrapperConfigurator.RequestStartup((container, pipeline, context) =>
                {
                    context.CurrentUser = new TestPrincipal(claims);
                });
            return configurableBootstrapperConfigurator;
        }

        private ConfigurableBootstrapper CreateBootstrapper(params Claim[] claims)
        {
            var configurableBootstrapper = new ConfigurableBootstrapper();
            this.ConfigureBootstrapper(configurableBootstrapper, claims);
            return configurableBootstrapper;
        }
    }
}