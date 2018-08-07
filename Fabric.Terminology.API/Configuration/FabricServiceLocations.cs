namespace Fabric.Terminology.API.Configuration
{
    using System;

    public class FabricServiceLocations
    {
        public FabricServiceLocations(Uri identityUri, Uri authorizationUri)
        {
            this.IdentityServiceUri = identityUri;
            this.AuthorizationServiceUri = authorizationUri;
        }

        public Uri IdentityServiceUri { get; }

        public Uri AuthorizationServiceUri { get; }
    }
}