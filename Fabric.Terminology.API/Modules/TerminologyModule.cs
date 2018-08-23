namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Linq;
    using System.Security.Claims;

    using CallMeMaybe;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Constants;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.API.Services;
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Querying;
    using Fabric.Terminology.SqlServer.Persistence.Ordering;

    using Nancy;
    using Nancy.Responses.Negotiation;

    using Serilog;

    public abstract class TerminologyModule<T> : NancyModule
    {
        private readonly UserAccessService userAccessService;

        protected TerminologyModule(string path, IAppConfiguration config, ILogger logger, UserAccessService userAccessService)
            : base(path)
        {
            this.userAccessService = userAccessService;
            this.Config = config;
            this.Logger = logger;
        }

        protected IAppConfiguration Config { get; }

        protected ILogger Logger { get; }

        protected Predicate<Claim> TerminologyReadClaim => claim =>
            claim.Type.Equals(Claims.Scope, StringComparison.OrdinalIgnoreCase) &&
            claim.Value.Equals(Scopes.ReadScope, StringComparison.OrdinalIgnoreCase);

        protected Predicate<Claim> TerminologyWriteClaim => claim =>
            claim.Type.Equals(Claims.Scope, StringComparison.OrdinalIgnoreCase) &&
            claim.Value.Equals(Scopes.WriteScope, StringComparison.OrdinalIgnoreCase);

        protected void RequireAccessorAuthorization() =>
            this.RequiresAuthorizationPermission(
                this.userAccessService,
                AuthorizationPermissions.Access,
                this.Config.AuthorizationServerSettings,
                this.TerminologyReadClaim);

        protected void RequirePublisherAuthorization() =>
            this.RequiresAuthorizationPermission(
                this.userAccessService,
                AuthorizationPermissions.Publish,
                this.Config.AuthorizationServerSettings,
                this.TerminologyWriteClaim);

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
                .WithHeader(HttpHeaderValues.Location, selfLink);
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
            var ordering = this.GetValueSetOrdering();

            return new PagerSettings
            {
                CurrentPage = skip == 0 ? 1 : skip + 1,
                ItemsPerPage = count == 0 ? this.Config.TerminologySqlSettings.DefaultItemsPerPage : count,
                OrderBy = ordering.FieldName,
                Direction = ordering.Direction
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

        protected Guid[] GetCodeSystems() =>
            this.CreateGuidParameterArray((string)this.Request.Query["$codesystems"]);

        protected IOrderingParameters GetValueSetOrdering()
        {
            var directive = this.CreateParameterArray((string)this.Request.Query["$orderBy"], ' ');
            switch (directive.Length)
            {
                case 2:
                    return ValueSetOrderingHelper.GetValidValueSetOrdering(directive[0], directive[1]);
                case 1:
                    return ValueSetOrderingHelper.GetValidValueSetOrdering(directive[0], SortDirection.Asc.ToString());
                case 0:
                default:
                    return new ValueSetOrderingParameters();
            }
        }

        protected string[] CreateParameterArray(string value, char splitChar = ',')
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<string>();
            }

            var cds = value.Split(splitChar);
            return cds.Select(cd => cd.Trim()).Where(cd => !cd.IsNullOrWhiteSpace()).ToArray();
        }

        private Guid[] CreateGuidParameterArray(string value) =>
            this.CreateParameterArray(value).Select(s => Maybe.From(Guid.Parse(s))).Values().ToArray();
    }
}