namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;

    using Nancy;
    using Nancy.Responses.Negotiation;

    using Serilog;

    using Constants = Fabric.Terminology.API.Constants;

    public abstract class TerminologyModule<T> : NancyModule
    {
        protected TerminologyModule(string path, IAppConfiguration config, ILogger logger)
            : base(path)
        {
            this.Config = config;
            this.Logger = logger;
        }

        protected IAppConfiguration Config { get; }

        protected ILogger Logger { get; }

        protected Negotiator CreateSuccessfulPostResponse(IIdentifiable model)
        {
            var uriBuilder = new UriBuilder(
                this.Request.Url.Scheme,
                this.Request.Url.HostName,
                this.Request.Url.Port ?? 80,
                $"{this.ModulePath}/{model.Identifier}");

            var selfLink = uriBuilder.ToString();

            return this.Negotiate.WithModel(model)
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader(Constants.HttpResponseHeaders.Location, selfLink);
        }

        protected Negotiator CreateFailureResponse(string message, HttpStatusCode statusCode)
        {
            var error = ErrorFactory.CreateError<T>(message, statusCode);
            return this.Negotiate.WithModel(error).WithStatusCode(statusCode);
        }

        protected IPagerSettings GetPagerSettings()
        {
            var skip = (int)this.Request.Query["$skip"];
            var count = (int)this.Request.Query["$top"];
            return new PagerSettings
            {
                CurrentPage = skip == 0 ? 1 : skip + 1,
                ItemsPerPage = count == 0 ? this.Config.TerminologySqlSettings.DefaultItemsPerPage : count
            };
        }

        protected object ParseValueSetGuidAndExecute(string valueSetGuidString, Func<Guid, object> getResponse)
        {
            if (!Guid.TryParse(valueSetGuidString, out Guid valueSetGuid))
            {
                return this.CreateFailureResponse(
                    $"The valueSetGuid parameter '{valueSetGuidString}' could not be parsed as a valid GUID",
                    HttpStatusCode.BadRequest);
            }

            return getResponse(valueSetGuid);
        }

        protected Guid[] GetCodeSystems()
        {
            return this.CreateGuidParameterArray((string)this.Request.Query["$codesystems"]);
        }

        private string[] CreateParameterArray(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return new string[] { };
            }

            var cds = value.Split(',');
            return cds.Select(cd => cd.Trim()).Where(cd => !cd.IsNullOrWhiteSpace()).ToArray();
        }

        private Guid[] CreateGuidParameterArray(string value)
        {
            return this.CreateParameterArray(value).Select(s => Maybe.From(Guid.Parse(s))).Values().ToArray();
        }
    }
}