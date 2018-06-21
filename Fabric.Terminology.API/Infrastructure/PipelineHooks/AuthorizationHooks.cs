namespace Fabric.Terminology.API.Infrastructure.PipelineHooks
{
    using System;

    using Catalyst.DosApi.Authorization.Compliance;

    using Fabric.Terminology.API.Services;

    using Nancy;

    public static class AuthorizationHooks
    {
        public static Func<NancyContext, Response> RequiresPermission(
            UserAccessService service,
            PermissionName permissionName)
        {
            var accessTask = service.UserHasAccess(permissionName);
            accessTask.Wait();

            return (ctx) => accessTask.Result
                ? null
                : new Response
                {
                    StatusCode = HttpStatusCode.Forbidden
                };
        }
    }
}