namespace Fabric.Terminology.API.Models
{
    using System;

    using FluentValidation.Results;

    using Nancy;

    // acquired from Fabric.Authorization.Domain
    public static class ErrorFactory
    {
        public static Error CreateError<T>(ValidationResult validationResult, HttpStatusCode statusCode)
        {
            var error = validationResult.ToError();
            error.Code = Enum.GetName(typeof(HttpStatusCode), statusCode);
            error.Target = typeof(T).Name;
            return error;
        }

        public static Error CreateError<T>(string message, HttpStatusCode statusCode)
        {
            var error = new Error
            {
                Code = Enum.GetName(typeof(HttpStatusCode), statusCode),
                Target = typeof(T).Name,
                Message = message
            };
            return error;
        }
    }
}
