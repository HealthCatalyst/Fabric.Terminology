namespace Fabric.Terminology.API.Modules
{
    using System;

    using Fabric.Terminology.API.Configuration;

    using Serilog;

    public abstract class ValueSetBackingItemModule<T> : TerminologyModule<T>
    {
        protected ValueSetBackingItemModule(string path, IAppConfiguration config, ILogger logger)
            : base(path, config, logger)
        {

            this.Get("/", _ => this.DoGetPaged(), null, "DoGetPaged");

            this.Get("/{valueSetGuid}", parameters => this.DoGetMultiple(this.GetValueSetGuids(parameters.valueSetGuid)), null, "DoGet");

            this.Post("/search/", _ => this.DoSearch(), null, "DoSearch");
        }

        protected abstract object DoGet(Guid valueSetGuid);

        protected abstract object DoGetMultiple(Guid[] valueSetGuids);

        protected abstract object DoGetPaged();

        protected abstract object DoSearch();
    }
}