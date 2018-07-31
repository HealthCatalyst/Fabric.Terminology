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
            PermissionName permissionName,
            int cacheDurationSeconds)
        {
            var accessTask = service.UserHasAccessAsync(permissionName, cacheDurationSeconds);
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