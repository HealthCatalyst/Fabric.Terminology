namespace Fabric.Terminology.API.Infrastructure.PipelineHooks
{
    using System;
    using System.Linq;

    using Fabric.Terminology.API.Models;

    using Nancy;
    using Nancy.Responses;

    public static class RequestHooks
    {
        public static readonly Func<NancyContext, Response> RemoveContentTypeHeaderForGet = context =>
        {
            // only check GET requests
            if (!context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var contentType = context.Request.Headers.ContentType;
            if (contentType == null)
            {
                return null;
            }

            // remove content-type header from the request
            var originalRequest = context.Request;
            var headers = originalRequest.Headers.ToDictionary(originalRequestHeader => originalRequestHeader.Key,
                originalRequestHeader => originalRequestHeader.Value);

            headers.Remove("Content-Type");

            var updatedRequest = new Request(
                originalRequest.Method,
                originalRequest.Url,
                originalRequest.Body,
                headers,
                originalRequest.UserHostAddress,
                originalRequest.ClientCertificate,
                originalRequest.ProtocolVersion);

            context.Request = updatedRequest;

            return null;
        };

        public static readonly Func<NancyContext, Response> ErrorResponseIfContentTypeMissingForPostPutAndPatch = context =>
            {
                // Only check POST and PUT requests
                if (!context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase)
                    && !context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase)
                    && !context.Request.Method.Equals("PATCH", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                var contentType = context.Request.Headers.ContentType;
                if (contentType.Type.ToString().Equals("application", StringComparison.OrdinalIgnoreCase) &&
                    (contentType.Subtype.ToString().Equals("json", StringComparison.OrdinalIgnoreCase) ||
                     contentType.Subtype.ToString().Equals("xml", StringComparison.OrdinalIgnoreCase)))
                {
                    return null;
                }

                // Invalid content type header specified so return a response to the client letting them know
                var error = new Error
                {
                    Code = Enum.GetName(typeof(HttpStatusCode), HttpStatusCode.UnsupportedMediaType),
                    Message = "Content-Type header must be application/json or application/xml when attempting a POST, PUT or PATCH."
                };
                return new JsonResponse(error, new DefaultJsonSerializer(context.Environment), context.Environment) { StatusCode = HttpStatusCode.BadRequest };
            };
    }
}